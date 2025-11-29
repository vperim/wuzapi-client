namespace WuzApiClient.EventDashboard.Helpers;

public static class PhoneNumberFormatter
{
    public static string Format(string? jid)
    {
        if (string.IsNullOrWhiteSpace(jid))
            return string.Empty;

        var atIndex = jid.IndexOf('@');
        if (atIndex <= 0)
            return jid;

        var local = jid[..atIndex];

        // Only format personal JIDs (@s.whatsapp.net) with pure numeric local part
        // Group JIDs (@g.us) and others should not be formatted as phone numbers
        if (!jid.EndsWith("@s.whatsapp.net", StringComparison.OrdinalIgnoreCase) ||
            !local.All(char.IsDigit))
        {
            return local;
        }

        var phone = local;

        if (phone.Length < 10)
            return "+" + phone;

        // Brazilian format (55 XX 9XXXX-XXXX or 55 XX XXXX-XXXX)
        if (phone.StartsWith("55") && phone.Length >= 12)
        {
            var country = phone[..2];
            var area = phone[2..4];
            var rest = phone[4..];

            return rest.Length switch
            {
                9 => $"+{country} {area} {rest[..5]}-{rest[5..]}",
                8 => $"+{country} {area} {rest[..4]}-{rest[4..]}",
                _ => "+" + phone
            };
        }

        // Generic international: just add + prefix
        return "+" + phone;
    }
}

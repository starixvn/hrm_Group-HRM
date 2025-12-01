using System;
using System.Globalization;
using Fanex.Data;

namespace HRM.Services.Helper;

public static class PayrollHelper
{
    public static decimal DecryptSalary(string encryptedSalary)
    {
        if (string.IsNullOrEmpty(encryptedSalary))
        {
            return 0;
        }

        try
        {
            var decryptedValue = Cryptography.DecryptPassword(encryptedSalary);
            return decimal.Parse(decryptedValue, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static string EncryptSalary(decimal salary)
    {
        return Cryptography.EncryptPassword(salary.ToString(CultureInfo.InvariantCulture));
    }

    public static string EncryptAndRound(decimal amount, int decimals = 0)
    {
        var roundedAmount = Math.Round(amount, decimals);
        return EncryptSalary(roundedAmount);
    }
}


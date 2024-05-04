using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class RandomUtil
{
    public static string GenerateRamdomStr()
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        int length = Random.Range(5, 30);
        string s = null, str = "I'm:";
        str += "0123456789";
        str += "abcdefghijklmnopqrstuvwxyz";
        str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        for (int i = 0; i < length; i++)
        {
            s += str.Substring(Random.Range(0, str.Length - 1), 1);
        }
        return s;
    }
}

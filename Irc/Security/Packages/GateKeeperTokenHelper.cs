﻿using System.Runtime.InteropServices;

namespace Irc.Security.Packages;

public class GateKeeperTokenHelper
{
    public static GateKeeperToken InitializeFromBytes(byte[] Data)
    {
        var AuthToken = new GateKeeperToken();
        var pBuf = Marshal.AllocHGlobal(Marshal.SizeOf(AuthToken));
        try
        {
            Marshal.Copy(Data, 0, pBuf, Marshal.SizeOf(AuthToken));
            AuthToken = Marshal.PtrToStructure<GateKeeperToken>(pBuf);
        }
        catch (Exception)
        {
        }
        finally
        {
            Marshal.FreeHGlobal(pBuf);
        }

        return AuthToken;
    }
}
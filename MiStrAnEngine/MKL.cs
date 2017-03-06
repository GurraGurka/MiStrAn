using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace MiStrAnEngine
{
    public static class MKL
    {
        public static int pardiso(IntPtr[] handle,
            ref int maxfct, ref int mnum,
            ref int mtype, ref int phase, ref int n,
            double[] a, int[] ia, int[] ja, int[] perm,
            ref int nrhs, int[] iparm, ref int msglvl,
            double[] b, double[] x, ref int error)
        {
            return PardisoNative.pardiso(handle,
                ref maxfct, ref mnum, ref mtype, ref phase, ref n,
                a, ia, ja, perm, ref nrhs, iparm, ref msglvl,
                b, x, ref error);
        }
    }


    /** Pardiso native declarations */
    [SuppressUnmanagedCodeSecurity]
    internal sealed class PardisoNative
    {
        private PardisoNative() { }

        [DllImport("mkl_rt.dll", CallingConvention = CallingConvention.Cdecl,
             ExactSpelling = true, SetLastError = false)]
        internal static extern int pardiso([In, Out] IntPtr[] handle,
            ref int maxfct, ref int mnum,
            ref int mtype, ref int phase, ref int n,
            [In] double[] a, [In] int[] ia, [In] int[] ja, [In] int[] perm,
            ref int nrhs, [In, Out] int[] iparm, ref int msglvl,
            [In, Out] double[] b, [Out] double[] x, ref int error);
    }

}

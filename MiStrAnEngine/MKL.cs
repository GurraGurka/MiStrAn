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

        public static void GeneralizedEigenSolver(ref char uplo, ref int n, double[] a, int[] ia, int[] ja, double[] b, int[] ib, int[] jb,
            int[] fpm, ref double epsout, ref int loops, ref double emin, ref double emax, ref int m0, double[] e, double[] x, ref int m,  double[] res, ref int info)
        {
            EigensolveNative.dfeast_scsrgv(ref uplo,ref  n, a, ia, ja, b, ib, jb, fpm, ref epsout, ref loops, ref emin, ref emax, ref m0, e, x,  ref m, res,  ref info);
        }


        /** CBLAS cblas_dgemm wrapper */
        public static void dgemm(int Order, int TransA, int TransB,
            int M, int N, int K, double alpha, double[] A, int lda,
            double[] B, int ldb, double beta, double[] C, int ldc)
        {
            CBLASNative.cblas_dgemm(Order, TransA, TransB, M, N, K,
                alpha, A, lda, B, ldb, beta, C, ldc);
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

    /** Eigensolver native declarations*/
    [SuppressUnmanagedCodeSecurity]
    internal sealed class EigensolveNative
    {

        [DllImport("mkl_rt.dll", CallingConvention = CallingConvention.Cdecl,
             ExactSpelling = true, SetLastError = false)]
        internal static extern void dfeast_scsrgv(ref char uplo, ref int n,[In] double[] a, [In] int[] ia, 
            [In] int[] ja,[In]double[] b, [In]int[] ib, [In]int[] jb, [In,Out]int[] fpm,ref double epsout, ref int loops,
            ref double emin, ref double emax, ref int m0,
            [In, Out]double[] e, [Out]double[] x, ref int m, [Out]double[] res, ref int info);
    }


    /** CBLAS wrappers */
    public sealed class CBLAS
    {
        private CBLAS() { }

        /** Constants for CBLAS_ORDER enum, file "mkl_cblas.h" */
        public sealed class ORDER
        {
            private ORDER() { }
            public static int RowMajor = 101;  /* row-major arrays */
            public static int ColMajor = 102;  /* column-major arrays */
        }

        /** Constants for CBLAS_TRANSPOSE enum, file "mkl_cblas.h" */
        public sealed class TRANSPOSE
        {
            private TRANSPOSE() { }
            public static int NoTrans = 111; /* trans='N' */
            public static int Trans = 112; /* trans='T' */
            public static int ConjTrans = 113; /* trans='C' */
        }

    }

    /** CBLAS native declarations */
    [SuppressUnmanagedCodeSecurity]
    internal sealed class CBLASNative
    {
        private CBLASNative() { }

        /** CBLAS native cblas_dgemm declaration */
        [DllImport("mkl_rt.dll", CallingConvention = CallingConvention.Cdecl,
             ExactSpelling = true, SetLastError = false)]
        internal static extern void cblas_dgemm(
            int Order, int TransA, int TransB, int M, int N, int K,
            double alpha, [In] double[] A, int lda, [In] double[] B, int ldb,
            double beta, [In, Out] double[] C, int ldc);
    }

}

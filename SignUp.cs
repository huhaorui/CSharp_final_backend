using backend;
using System;

namespace backend
{
    public enum Status
    {
        SUCCESSFUL = 0,
        FAILED = 1
    }

    public class SignUp
    {
        readonly Status status;
        public SignUp()
        {
            status = 0;
        }
    }
}

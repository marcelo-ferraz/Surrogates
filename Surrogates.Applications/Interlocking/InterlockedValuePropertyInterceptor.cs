﻿
namespace Surrogates.Applications.Interlocking
{
    public class InterlockedValuePropertyInterceptor
        : InterlockedPropertyInterceptor
    {
        protected override object GetField(object field)
        {
            return field;
        }
    }
}
//  ©️ 2024 by RF@EggNine.com All Rights Reserved
﻿using OpenQA.Selenium.Internal;
using System.Drawing;

namespace Eggnine..XUnit.SkipAttributes
{
    public abstract class SkipIf : Attribute
    {
        /// <summary>
        /// Returns true if the test should be skipped
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract bool Matches(TrashContext ctx);
        
        /// <summary>
        /// Provides a reason that the test was skipped
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract string Reason(TrashContext ctx);

        public void True(TrashContext ctx)
        {
            if(Matches(ctx))
            {
                string reason = Reason(ctx);
                Console.WriteLine($"skipping test {ctx.ClassName}.{ctx.TestName} because {reason}");
                throw new SkipException(reason);
            }
        }
    }
}

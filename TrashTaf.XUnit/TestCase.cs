﻿// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
namespace TrashTaf.XUnit
{
    public class TestCase : Attribute
    {
        private int _id;
        public TestCase(int id)
        {
            _id = id;
        }
    }
}

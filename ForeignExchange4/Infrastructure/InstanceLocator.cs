﻿namespace ForeignExchange4.Infrastructure
{
    using ViewModels;

    public class InstanceLocator
    {
        public MainViewModel Main
        {
            get;
            set;
        }

        public InstanceLocator()
        {
            Main = new MainViewModel();
        }
    }
}

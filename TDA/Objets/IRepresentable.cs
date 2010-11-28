namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;

    interface IRepresentable
    {
        IVisible representation { get; set; }
    }
}

﻿namespace EventPlus.Server.Application.ViewModels
{
    public class SectorViewModel
    {
        public int IdSector { get; set; }
        public int FkEventLocationidEventLocation { get; set; }
        public string? Name { get; set; }
        public List<int> IdSeating { get; set; } = new List<int>();
        public List<int> IdSectorPrice { get; set; } = new List<int>();
    }
}

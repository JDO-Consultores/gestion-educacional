namespace GestionColegios.Responses
{
    public class RegionesResponse
    {
        public int ID { get; set; }
        public int NroRegion { get; set; }
        public string Simbologia { get; set; }
        public string Region { get; set; }
        public bool IsActive { get; set; }

        public string FullData => $"{Simbologia} - {Region}";
    }
}

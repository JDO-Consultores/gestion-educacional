namespace GestionColegios.Responses
{
    public class ComunasResponse
    {
        public int ID { get; set; }
        public int RegionID { get; set; }
        public string Comuna { get; set; }
        public bool IsActive { get; set; }
        public RegionesResponse Region { get; set; }

        public string Text => Comuna;

    }
}
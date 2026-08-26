namespace Gestionador.Reports.Responses
{
    public class SectoresResponse
    {
        public int ID { get; set; }
        public string Sector { get; set; }
        public bool IsActive { get; set; }
        public string Text => Sector;
    }
}
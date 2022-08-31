namespace e_citaonica_api.TransferModels
{
    public class UserRegister
    {
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Lozinka { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Indeks { get; set; }
        public int Godina { get; set; }
        public int ModulId { get; set; }
        public bool JeProfesor { get; set; } = false;

    }
}

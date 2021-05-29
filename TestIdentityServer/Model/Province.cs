using System.Collections.Generic;

namespace TestIdentityServer.Model
{
    public class Province
    {
        public static readonly Province PinarDelRio = new Province(1, "Pinar del Río");
        public static readonly Province Artemisa = new Province(2, "Artemisa");
        public static readonly Province LaHabana = new Province(3, "La Habana");
        public static readonly Province Mayabeque = new Province(4, "Mayabeque");
        public static readonly Province Matanzas = new Province(5, "Matanzas");
        public static readonly Province Cienfuegos = new Province(6, "Cienfuegos");
        public static readonly Province VillaClara = new Province(7, "Villa Clara");
        public static readonly Province SanctiSpíritus = new Province(8, "Sancti Spíritus");
        public static readonly Province CiegoDeÁvila = new Province(9, "Ciego de Ávila");
        public static readonly Province Camaguey = new Province(10, "Camagüey");
        public static readonly Province LasTunas = new Province(11, "Las Tunas");
        public static readonly Province Granma = new Province(12, "Granma");
        public static readonly Province Holguin = new Province(13, "Holguín");
        public static readonly Province SantiagoDeCuba = new Province(14, "Santiago de Cuba");
        public static readonly Province Guantanamo = new Province(15, "Guantánamo");
        public static readonly Province IslaDeLaJuventud = new Province(16, "Municipio Especial Isla de la Juventud");

        public int Id { get; private set; }
        public string Name { get; private set; }

        public Province(int id, string name) 
        {
            Id = id;
            Name = name;
        }

        public static List<Province> GetAll()
        {
            return new List<Province>()
            {
                PinarDelRio,
                Artemisa,
                LaHabana,
                Mayabeque,
                Matanzas,
                Cienfuegos,
                VillaClara,
                SanctiSpíritus,
                CiegoDeÁvila,
                Camaguey,
                LasTunas,
                Granma,
                Holguin,
                SantiagoDeCuba,
                Guantanamo,
                IslaDeLaJuventud,
            };
        }

    }
}

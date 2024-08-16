using System;
using System.IO;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace Examen3.Models
{
    public class Nota
    {
        [JsonProperty("id_nota")]
        public int IdNota { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("fecha")]
        public double Fecha { get; set; }

        [JsonProperty("photo_record")]
        public byte[] PhotoRecord { get; set; }


        [JsonIgnore]  // No se serializa en JSON, solo se usa en la UI
        public ImageSource PhotoSource
        {
            get
            {
                if (PhotoRecord == null || PhotoRecord.Length == 0)
                    return null;

                return ImageSource.FromStream(() => new MemoryStream(PhotoRecord));
            }
        }
    }
}

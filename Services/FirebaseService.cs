using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examen3.Models;

namespace Examen3.Services
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseService()
        {
            _firebaseClient = new FirebaseClient("https://examen3erp-default-rtdb.firebaseio.com/");
        }

        public async Task AddNotaAsync(Nota nota)
        {
            await _firebaseClient
                .Child("notas")
                .PostAsync(JsonConvert.SerializeObject(nota));
        }

        public async Task<List<Nota>> GetNotasAsync()
        {
            var notas = await _firebaseClient
                .Child("notas")
                .OnceAsync<Nota>();

            return notas.Select(n => n.Object).ToList();
        }

        public async Task UpdateNotaAsync(Nota nota)
        {
            var toUpdateNota = (await _firebaseClient
                .Child("notas")
                .OnceAsync<Nota>())
                .FirstOrDefault(a => a.Object.IdNota == nota.IdNota);

            await _firebaseClient
                .Child("notas")
                .Child(toUpdateNota.Key)
                .PutAsync(JsonConvert.SerializeObject(nota));
        }

        public async Task EliminarNotaAsync(int idNota)
        {
            var toDeleteNota = (await _firebaseClient
                .Child("notas")
                .OnceAsync<Nota>())
                .FirstOrDefault(a => a.Object.IdNota == idNota);

            if (toDeleteNota != null)
            {
                await _firebaseClient.Child("notas").Child(toDeleteNota.Key).DeleteAsync();
            }
        }

        public async Task<List<Nota>> LeerNotasAsync()
        {
            var notas = await _firebaseClient
                .Child("notas")
                .OnceAsync<Nota>();

            return notas.Select(n => n.Object).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DADStorm{

	[Serializable()]
	public class Tuple : EventArgs {

		private List<string> items;
        public DateTime date;
        public string id;
        public IReplica origin;
        public Boolean sent = false;
        public Tuple father = null;

        public string filename;
        public int line;

        public void init() {
            this.id = GenerateIdentifier();
            this.date = DateTime.Now;
        }

		public Tuple(params string[] args){
			items = new List<string>(args);
            init();
		}

        public Tuple() {
            items = new List<string>();
            init();
        }

        public Tuple(List<string> input) {
            this.items = input;
            init();
        }

        public Tuple(IList<string> input) {
            this.items = new List<string>();
            foreach(string item in input)
                this.items.Add(item);
            init();
        }

        public List<string> toList() {
            return items;
        }

        public void Merge(Tuple t) {
            for (int i = 1; i <= t.Count(); i++)
                items.Add(t.Get(i));
            init();
        }

        public string Get(int index){
			return items[index-1];
		}

		public Boolean Contains(string item){
			return items.Contains(item);
		}

        public void RemoveField(int index) {
            items.RemoveAt(index - 1);
            init();
        }

        public int Count() {
            return items.Count;
        }

		public void Add(string item){
			items.Add(item);
            init();
		}

		public override string ToString(){
			return string.Join(",", items.ToArray());
		}

        private string RandomString() {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < 20; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * new Random((int)DateTime.Now.Ticks).NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }


        private static string GenerateIdentifier() {
            int length = 50;
            char[] AvailableCharacters = {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(randomData);
            }

            for (int idx = 0; idx < identifier.Length; idx++) {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            return new string(identifier);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    interface gb_word {
        void Set(ushort value);
        ushort Get();
    }
    class gb_combinationWord : gb_word {
        public ref byte a;
        public ref byte b;
        public gb_combinationWord(ref byte a, ref byte b) {
            this.a = ref a;
            this.b = ref b;
        }
        public ushort Get() {
            throw new NotImplementedException();
        }

        public void Set(ushort value) {
            
        }
    }
    class gb_normalWord : gb_word {
        ushort word;
        public gb_normalWord() {
            word = 0;
        }
        public ushort Get() {
            return word;
        }

        public void Set(ushort value) {
        }
    }

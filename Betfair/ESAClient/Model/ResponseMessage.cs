using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Text;

namespace Betfair.ESAClient.Model
{
    [DataContract]
    public class ResponseMessage : IEquatable<ResponseMessage>
    {
        public ResponseMessage(string op = null, int? Id = null) 
        { 
            this.op = op;
            this.id = Id;
        }
        [DataMember(Name = "op", EmitDefaultValue = false)]
        public string op { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? id { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("class ResponseMessage {\n");

            builder.Append(" Op: ")
                .Append(op)
                .Append("\n");

            builder.Append(" Id: ")
                .Append(id)
                .Append("\n");

            builder.Append("}\n");

            return builder.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResponseMessage);
        }

        public bool Equals(ResponseMessage? other)
        {
            if (other == null) return false;

            return (op == other.op || op != null && op.Equals(other.op)) && 
                (id == other.id || id != null && id.Equals(other.id));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 41;

                if (op != null)
                    hash = hash * 59 + op.GetHashCode();

                if (id != null)
                    hash = hash * 59 + id.GetHashCode();

                return hash;
            }
        }
    }
}

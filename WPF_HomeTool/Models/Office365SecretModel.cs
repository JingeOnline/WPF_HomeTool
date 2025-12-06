using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public class Office365SecretModel
    {
        public string ClientId {  get; set; }
        public string TenantId {  get; set; }
        public string Scopes {  get; set; }
        public string SecretValue {  get; set; }
        public string UserName { get; set; }
    }
}

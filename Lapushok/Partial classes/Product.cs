using Lapushok.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lapushok.Components
{
    public partial class Product
    {
        public string allmaterials
        {
            get
            {
                string result = null;
                List<ProductMaterial> material = App.db.ProductMaterial.Where(x => x.ProductId == Id).ToList();
                foreach (ProductMaterial materialItem in material)
                {
                    result += materialItem.Material.Name;
                }
                return result;

            }

        }
    }
}

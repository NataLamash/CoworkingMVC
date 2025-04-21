using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    [Table("coworking_space_images")]
    public class CoworkingSpaceImage
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("coworking_space_id")]
        public int CoworkingSpaceId { get; set; }
        public CoworkingSpace CoworkingSpace { get; set; } = null!;

        [Column("file_path")]
        public string FilePath { get; set; } = null!;
    }
}

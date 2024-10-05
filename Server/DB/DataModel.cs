using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB
{
    [Table("Setting")]
    public class Setting
    {
        [Key]
        [Column("steamId")]
        public ulong steamId { get; set; }

        [Column("instanceId")]
        public int instanceId { get; set; }

        [Column("mouseSensitivity")]
        public float mouseSensitivity { get; set; }

        [Column("isFullScreen")]
        public bool isFullScreen { get; set; }

        [Column("displayQuality")]
        public int displayQuality { get; set; }

        [Column("width")]
        public int width { get; set; }

        [Column("height")]
        public int height { get; set; }
    }
}

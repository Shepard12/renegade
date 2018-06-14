using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ren.Models;

namespace ren.Code
{
	public class PhotoModel : List<Photos>
	{
        private IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoModel"/> class.
        /// </summary>
        public PhotoModel(string folder, IHostingEnvironment environment)
		{
            _hostingEnvironment = environment;

            string path = Path.Combine(_hostingEnvironment.WebRootPath, folder);
            var di = new DirectoryInfo(path);

			foreach (var file in di.EnumerateFiles("*.jpg", SearchOption.TopDirectoryOnly))
			{
				var p = new Photos(string.Concat(folder, file.Name), Path.GetFileNameWithoutExtension(file.Name));
				Add(p);
			}
		}
	}
}
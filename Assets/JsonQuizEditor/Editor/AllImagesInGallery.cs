// Image gallery.
using System.Collections.Generic;
using UnityEngine;

namespace JsonQuizEditor.Scripts
{    
    [System.Serializable]
    public class AllImagesInGallery
    {
        public List<Texture2D> imageTexture = null; // List of textures.
        public List<bool> isTexture = null; // true = yes texture loaded , false = texture was not loaded.
        public List<string> image = null; // List of image names.

        // Initialize variables.
        public AllImagesInGallery () {
            if (imageTexture == null) {
                imageTexture = new List<Texture2D>();
            }
            if (isTexture == null) {
                isTexture = new List<bool>();
            }                         
            if (image == null) {
                image = new List<string>();
            }                                                                      
        }
    }
}
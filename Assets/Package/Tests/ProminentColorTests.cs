using System.Collections;
using System.Collections.Generic;
using com.mukarillo.prominentcolor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ProminentColorTests {
        
        private Texture2D texture2D;
        
        [SetUp]
        public void Setup() {
            Texture2D tex = GameObject.Instantiate(Resources.Load<Texture2D>("colorgrid"));
            Debug.Log("tex : " + tex);
            texture2D = new Texture2D(tex.width,tex.height);
            texture2D.ReadPixels(new Rect(0, 0, tex.width,tex.height), 0, 0);
            texture2D.Apply();
        }
        
        [Test]
        public void GetColors32_Success() {
            List<Color32> colorList = null;
            
            colorList = ProminentColor.GetColors32FromImage(texture2D,
                3,
                85f,
                5,
                10f);
            
            Assert.NotNull(colorList);
            Assert.Greater(colorList.Count, 0);
        }
        
        [Test]
        public void GetColors32_InvalidImage_Failed() {
            List<Color32> colorList = null;
            
            
            Assert.That(() => colorList = ProminentColor.GetColors32FromImage(null,
                3,
                85f,
                5,
                10f), 
                Throws.TypeOf<System.Exception>());
        }
        
        [Test]
        public void GetColors32_InvalidData_Failed() {
            List<Color32> colorList = null;
            
            Assert.That(() => colorList = ProminentColor.GetColors32FromImage(texture2D,
                0,
                85f,
                10000,
                0f), 
                Throws.TypeOf<System.Exception>());
        }
        
        [Test]
        public void GetHexColors_Success() {
            List<string> hexList = null;
            
            hexList = ProminentColor.GetHexColorsFromImage(texture2D,
                3,
                85f,
                5,
                10f);
            
            Assert.NotNull(hexList);
            Assert.Greater(hexList.Count, 0);
        }
        
        [Test]
        public void GetHexColors_InvalidImage_Failed() {
            List<string> hexList = null;
            
            Assert.That(() => hexList = ProminentColor.GetHexColorsFromImage(null,
                3,
                85f,
                5,
                10f), 
                Throws.TypeOf<System.Exception>());
        }
    }
}

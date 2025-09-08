////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace FronkonGames.Artistic.Sharpen
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Sharpen
  {
    /// <summary> Sharpen algorithms used. </summary>
    public enum Algorithm
    {
      /// <summary>
      /// It applies a blurring effect to the original pixel by incorporating the neighboring pixels, and subsequently,
      /// it subtracts this blur to enhance the image's sharpness. This process is performed in the luma channel to
      /// prevent color artifacts and offers the option to restrict the maximum sharpening effect,
      /// thereby reducing or mitigating halo artifacts. This is similar to using Unsharp Mask in Photoshop.
      /// </summary>
      Luma,

      /// <summary>
      /// It dynamically regulates the sharpening intensity on a per-pixel basis to achieve a uniform level of
      /// sharpness throughout the entire image. Regions in the original image that are already sharp receive a milder
      /// sharpening treatment, whereas regions with less inherent detail undergo a more pronounced sharpening process.
      /// This approach results in an enhanced overall visual sharpness with a reduced occurrence of unwanted artifacts.
      /// </summary>
      ContrastAdaptive,

      /// <summary>
      /// Uses the Laplacian operator to detect edges and enhance them. This algorithm is very effective for edge
      /// enhancement and provides precise control over the sharpening process through different kernel sizes.
      /// </summary>
      Laplacian,

      /// <summary>
      /// Uses edge detection to apply sharpening only where needed, preventing over-sharpening of smooth areas.
      /// This approach provides intelligent sharpening that adapts to the image content.
      /// </summary>
      EdgeAware,

      /// <summary>
      /// Combines spatial and range filtering for edge-preserving sharpening. This algorithm preserves edges while
      /// smoothing noise, making it ideal for images with noise or texture that should be preserved.
      /// </summary>
      Bilateral,
    }

    /// <summary> Blur patterns used with the Luma algorithm. </summary>
    public enum LumaPattern
    {
      /// <summary> Only two texture fetches. Faster but slightly lower quality. </summary>
      Fast,

      /// <summary> Four texture fetches. </summary>
      Normal,

      /// <summary> Four texture fetches, less sensitive to noise but also to fine details. </summary>
      Wider,

      /// <summary> Four texture fetches, diamond-shaped. A slightly more aggresive look.  </summary>
      Pyramid,
    }

    /// <summary> Kernel types used with the Laplacian algorithm. </summary>
    public enum LaplacianKernel
    {
      /// <summary> 3x3 kernel. Fastest but less precise. </summary>
      Kernel3x3,

      /// <summary> 5x5 kernel. Good balance of quality and performance. </summary>
      Kernel5x5,

      /// <summary> 7x7 kernel. Highest quality but more expensive. </summary>
      Kernel7x7,
    }

    /// <summary> Edge detection methods used with the Edge-Aware algorithm. </summary>
    public enum EdgeDetectionMethod
    {
      /// <summary> Sobel edge detection. Good for general purpose edge detection. </summary>
      Sobel,

      /// <summary> Canny edge detection. More precise but computationally expensive. </summary>
      Canny,

      /// <summary> Laplacian edge detection. Simple and fast. </summary>
      Laplacian,
    }
  }
}

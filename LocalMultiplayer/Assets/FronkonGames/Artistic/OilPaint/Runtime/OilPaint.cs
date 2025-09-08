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
using UnityEngine;

namespace FronkonGames.Artistic.OilPaint
{
  /// <summary> Oil Paint misc. </summary>
  public static class OilPaint
  {
    /// <summary> Detail algorithms. </summary>
    public enum Detail
    {
      /// <summary> Node (default). </summary>
      None,

      /// <summary> Sharpen algorithm. </summary>
      Sharpen,

      /// <summary> Emboss algorithm. </summary>
      Emboss,
    }

    /// <summary> Default depth curve. </summary>
    public static readonly AnimationCurve DefaultDepthCurve = new()
    {
      keys = new Keyframe[]
      {
        new(0.0f, 1.0f),
        new(0.75f, 1.0f),
        new(1.0f, 0.25f),
      }
    };
  }
}

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

namespace FronkonGames.Artistic.Photo
{
  /// <summary> Vignette types. </summary>
  public enum Vignettes
  {
    // No vignette.
    None,

    // Rectangular.
    Rectangular,

    // Circular.
    Circular,
  }

  /// <summary> Film stock. </summary>
  public enum Films
  {
    // No film.
    None,

    // Agfa Vista 400: cool color balance, strong blue/green response, budget film look.
    Agfa_Vista_400,

    // Cinestill 800T: red halation, cinematic look.
    Cinestill_800T,

    // Fuji C200: cool tones, blue emphasis.
    Fuji_C200,

    // Fuji Velvia 50: high saturation slide film, punchy contrast, enhanced red/blue response.
    Fuji_Velvia_50,

    // Fuji Pro 400H: soft, natural skin tones and neutral gray balance.
    Fuji_Pro_400H,

    // Kodak Gold 200: golden hues, green boost.
    Kodak_Gold_200,

    // Kodak Portra 400: Soft highlight rolloff, enhanced greens for skin tones.
    Kodak_Portra_400,

    // Kodak Ektar 100: Ultra-vivid colors, high contrast, infrared-ready base.
    Kodak_Ektar_100,

    // Polaroid 600: warm tones, strong vignette.
    Polaroid_600,

    // Lomography Color 800: heavy grain, cyan shift, crushed shadows.
    Lomography_Color_800,

    // ORWO UT18: cinema film stock, green/magenta bias, soft highlight shoulder.
    ORWO_UT18,

    // Ilford HP5: black and white.
    Ilford_HP5_BW,
  }
}

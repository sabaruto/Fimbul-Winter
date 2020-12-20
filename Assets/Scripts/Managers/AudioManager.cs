using Base_Classes;

namespace Managers
{
  /// <summary>
  ///     This class manages all the different sounds that occurs and plays them
  ///     when needed
  /// </summary>
  public class AudioManager : SoundManager
    {
        private Sound mainTheme;

        /// <summary>
        ///     Plays the given theme, stopping the previous one
        /// </summary>
        public void ChangeTheme(string name)
        {
            var s = FindSound(name);

            if (s == null) return;

            // Stopping the previous song
            mainTheme?.Stop();

            // Starting the new scene
            mainTheme = s;
            s.Play();
        }
    }
}
using UnityEngine;

namespace Base_Classes
{
  /// <summary>
  ///   Makes the class useful for basic infromation showing
  /// </summary>
  public interface IInformation
  {
    string Name { get; }
    Sprite Sprite { get; }
  }
}
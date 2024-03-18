
namespace d4160.Characters
{
    public interface ICharacterAnimator
    {
        void PlayState(int index);
        void PlayTransition(int index, bool randomizeBlend = false, int maxBlendIndex = 1, float delay = 10f);

        bool IsInState(int index);
    }
}
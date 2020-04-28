using uMIDI.IO;

namespace uMIDI.Transform
{
    public interface ITransform
    {
        void Apply(Region region);
    }
}

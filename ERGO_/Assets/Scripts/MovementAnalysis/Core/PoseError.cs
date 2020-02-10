namespace Voxar
{
    public struct PoseError
    {
        public BasePlanes plane;
        public BoneType[] boneTypes;
        public float errorMargin;

        public PoseError(BoneType first, BoneType second, float error)
        {
            plane = BasePlanes.Frontal;
            boneTypes = new BoneType[2];
            boneTypes[0] = first;
            boneTypes[1] = second;
            errorMargin = error;
        }

        public PoseError(BasePlanes reference, BoneType first, BoneType second, float error)
        {
            plane = reference;
            boneTypes = new BoneType[2];
            boneTypes[0] = first;
            boneTypes[1] = second;
            errorMargin = error;
        }
    }
}

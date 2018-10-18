#if FEAT_COMPILER
namespace ProtoBuf.Compiler
{
    internal delegate void ProtoSerializer(object value, ProtoWriter dest);
    internal delegate object MessageDecodeerializer(object value, ProtoReader source);
}
#endif
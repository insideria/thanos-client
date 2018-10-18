namespace Thanos.Resource
{
    public enum ResourceRequestType
    {
        LOAD,
        UNLOAD,
        LOADLEVEL,
        UNLOADLEVEL,
    }

    class ResourceRequest
    {
        internal string mFileName;              //请求资源相对Assets/完整路径名称
        internal ResourceType mResourceType;
        internal ResourcesManager.HandleFinishLoad mHandle;
        internal ResourcesManager.HandleFinishLoadLevel mHandleLevel;
        internal ResourcesManager.HandleFinishUnLoadLevel mHandleUnloadLevel;
        internal ResourceRequestType mRequestType;
        internal ResourceAsyncOperation mResourceAsyncOperation;

        internal ResourceRequest(string fileName, ResourceType resourceType, ResourcesManager.HandleFinishLoad handle, ResourceRequestType requestType, ResourceAsyncOperation operation)
        {
            mFileName = fileName;
            mResourceType = resourceType;
            mHandle = handle;
            mRequestType = requestType;
            mResourceAsyncOperation = operation;
        }

        internal ResourceRequest(string fileName, ResourceType resourceType, ResourcesManager.HandleFinishLoadLevel handle, ResourceRequestType requestType, ResourceAsyncOperation operation)
        {
            mFileName = fileName;
            mResourceType = resourceType;
            mHandleLevel = handle;
            mRequestType = requestType;
            mResourceAsyncOperation = operation;
        }
    }
}
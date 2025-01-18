public static class EnumerableExtension{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> list ){
        if(list is null || !list.Any()){
            return true;
        }else{
            return false;
        }
    }
}
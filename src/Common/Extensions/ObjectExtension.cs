namespace DEA.Common.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Removes all markdown characters from a string and adds "**" to the front and back.
        /// </summary>
        /// <returns>String with the discord bold markdown.</returns>
        public static string Boldify(this object obj)
        {
            return $"**{obj.ToString().Replace("*", string.Empty).Replace("_", string.Empty).Replace("~", string.Empty).Replace("`", string.Empty)}**";
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace Etier.IconHelper
{
    class IconRead
    {
        public enum IconSize
        {
            Large = &H0,//'32x32
            Small = &H1,//'16x16
            ExtraLarge = &H2,//'48x48
            Jumbo = &H4//'256x256
        }
    }
    public class IconHelper
    {
    private const SHGFI_ICON  UInteger = &H100;
    private const SHGFI_LARGEICON   UInteger = &H0;
    private const SHGFI_SMALLICON   UInteger = &H1;
    private const SHGFI_USEFILEATTRIBUTES   UInteger = &H10;
    private const SHGFI_ICONLOCATION   UInteger = &H1000; 'The name of the file containing the icon is copied to the szDisplayName member of the structure specified by psfi. The icon's index is copied to that structure's iIcon member.
    private const SHGFI_SYSICONINDEX   UInteger = &H4000;
    private const SHIL_JUMBO   UInteger = &H4;
    private const SHIL_EXTRALARGE   UInteger = &H2;
    private const ILD_TRANSPARENT   UInteger = &H1;
    private const ILD_IMAGE   UInteger = &H20;
    private const FILE_ATTRIBUTE_NORMAL   UInteger = &H80;

    < DllImport("shell32.dll", EntryPoint:= "#727") > _
    private Shared Function SHGetImageList(ByVal iImageList   Integer, ByRef riid   Guid, ByRef ppv   IImageList)   Integer
    End Function

    <DllImport("shell32.dll", EntryPoint:="SHGetFileInfoW", CallingConvention:=CallingConvention.StdCall)> _
    private Shared Function SHGetFileInfoW(<Marshal (UnmanagedType.LPWStr)> ByVal pszPath   String, ByVal dwFileAttributes   UInteger, ByRef psfi   SHFILEINFOW, ByVal cbFileInfo   Integer, ByVal uFlags   UInteger)   Integer
    End Function

    <DllImport("shell32.dll", EntryPoint:="SHGetFileInfoW", CallingConvention:=CallingConvention.StdCall)> _
    private Shared Function SHGetFileInfoW(ByVal pszPath   IntPtr, ByVal dwFileAttributes   UInteger, ByRef psfi   SHFILEINFOW, ByVal cbFileInfo   Integer, ByVal uFlags   UInteger)   Integer
    End Function

    <DllImport("user32.dll", EntryPoint:="DestroyIcon")> _
    private Shared Function DestroyIcon(ByVal hIcon   IntPtr)  <Marshal (UnmanagedType.Bool)> Boolean
    }

   <StructLayout(LayoutKind.Sequential)> _
   private Structure RECT
        public left, top, right, bottom   Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.[Unicode])> _
    private Structure SHFILEINFOW
        public hIcon   System.IntPtr
        public iIcon   Integer
        public dwAttributes   UInteger
        <Marshal (UnmanagedType.ByValTStr, Sizeconst:=260)> public szDisplayName   String
        <Marshal (UnmanagedType.ByValTStr, Sizeconst:=80)> public szTypeName   String
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    private Structure IMAGELISTDRAWPARAMS
        public cbSize   Integer
        public himl   IntPtr
        public i   Integer
        public hdcDst   IntPtr
        public x   Integer
        public y   Integer
        public cx   Integer
        public cy   Integer
        public xBitmap   Integer
        public yBitmap   Integer
        public rgbBk   Integer
        public rgbFg   Integer
        public fStyle   Integer
        public dwRop   Integer
        public fState   Integer
        public Frame   Integer
        public crEffect   Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    private Structure IMAGEINFO
        public hbmImage   IntPtr
        public hbmMask   IntPtr
        public Unused1   Integer
        public Unused2   Integer
        public rcImage   RECT
    End Structure

    <ComImport(), Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    private Interface IImageList
        <PreserveSig()> Function Add(ByVal hbmImage   IntPtr, ByVal hbmMask   IntPtr, ByRef pi   Integer)   Integer
        <PreserveSig()> Function ReplaceIcon(ByVal i   Integer, ByVal hicon   IntPtr, ByRef pi   Integer)   Integer
        <PreserveSig()> Function SetOverlayImage(ByVal iImage   Integer, ByVal iOverlay   Integer)   Integer
        <PreserveSig()> Function Replace(ByVal i   Integer, ByVal hbmImage   IntPtr, ByVal hbmMask   IntPtr)   Integer
        <PreserveSig()> Function AddMasked(ByVal hbmImage   IntPtr, ByVal crMask   Integer, ByRef pi   Integer)   Integer
        <PreserveSig()> Function Draw(ByRef pimldp   IMAGELISTDRAWPARAMS)   Integer
        <PreserveSig()> Function Remove(ByVal i   Integer)   Integer
        <PreserveSig()> Function GetIcon(ByVal i   Integer, ByVal flags   Integer, ByRef picon   IntPtr)   Integer
        <PreserveSig()> Function GetImageInfo(ByVal i   Integer, ByRef pImageInfo   IMAGEINFO)   Integer
        <PreserveSig()> Function Copy(ByVal iDst   Integer, ByVal punkSrc   IImageList, ByVal iSrc   Integer, ByVal uFlags   Integer)   Integer
        <PreserveSig()> Function Merge(ByVal i1   Integer, ByVal punk2   IImageList, ByVal i2   Integer, ByVal dx   Integer, ByVal dy   Integer, ByRef riid   Guid, ByRef ppv   IntPtr)   Integer
        <PreserveSig()> Function Clone(ByRef riid   Guid, ByRef ppv   IntPtr)   Integer
        <PreserveSig()> Function GetImageRect(ByVal i   Integer, ByRef prc   RECT)   Integer
        <PreserveSig()> Function GetIconSize(ByRef cx   Integer, ByRef cy   Integer)   Integer
        <PreserveSig()> Function SetIconSize(ByVal cx   Integer, ByVal cy   Integer)   Integer
        <PreserveSig()> Function GetImageCount(ByRef pi   Integer)   Integer
        <PreserveSig()> Function SetImageCount(ByVal uNewCount   Integer)   Integer
        <PreserveSig()> Function SetBkColor(ByVal clrBk   Integer, ByRef pclr   Integer)   Integer
        <PreserveSig()> Function GetBkColor(ByRef pclr   Integer)   Integer
        <PreserveSig()> Function BeginDrag(ByVal iTrack   Integer, ByVal dxHotspot   Integer, ByVal dyHotspot   Integer)   Integer
        <PreserveSig()> Function EndDrag()   Integer
        <PreserveSig()> Function DragEnter(ByVal hwndLock   IntPtr, ByVal x   Integer, ByVal y   Integer)   Integer
        <PreserveSig()> Function DragLeave(ByVal hwndLock   IntPtr)   Integer
        <PreserveSig()> Function DragMove(ByVal x   Integer, ByVal y   Integer)   Integer
        <PreserveSig()> Function SetDragCursorImage(ByRef punk   IImageList, ByVal iDrag   Integer, ByVal dxHotspot   Integer, ByVal dyHotspot   Integer)   Integer
        <PreserveSig()> Function DragShowNolock(ByVal fShow   Integer)   Integer
        <PreserveSig()> Function GetDragImage(ByRef ppt   Point, ByRef pptHotspot   Point, ByRef riid   Guid, ByRef ppv   IntPtr)   Integer
        <PreserveSig()> Function GetItemFlags(ByVal i   Integer, ByRef dwFlags   Integer)   Integer
        <PreserveSig()> Function GetOverlayImage(ByVal iOverlay   Integer, ByRef piIndex   Integer)   Integer
    End Interface

    public Shared Function GetIconFrom(ByVal PathName   String, ByVal IcoSize   IconSize, ByVal UseFileAttributes   Boolean)   Icon
        Dim ico   Icon = Nothing
        Dim shinfo   New SHFILEINFOW()
        Dim flags   UInteger = SHGFI_SYSICONINDEX

        If UseFileAttributes Then flags = (flags Or SHGFI_USEFILEATTRIBUTES)

        If SHGetFileInfoW(PathName, FILE_ATTRIBUTE_NORMAL, shinfo, Marshal.SizeOf(shinfo), flags) = 0 Then
            Throw New IO.FileNotFoundException()
        End If

        Dim iidImageList   New Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")
        Dim iml   IImageList = Nothing
        SHGetImageList(IcoSize, iidImageList, iml)

        If iml IsNot Nothing Then
            Dim hIcon   IntPtr = IntPtr.Zero
            iml.GetIcon(shinfo.iIcon, ILD_IMAGE, hIcon)
            ico = CType(Icon.FromHandle(hIcon).Clone, Icon)
            DestroyIcon(hIcon)
            If Not ico.ToBitmap.PixelFormat = Imaging.PixelFormat.Format32bppArgb Then
                ico.Dispose()
                iml.GetIcon(shinfo.iIcon, ILD_TRANSPARENT, hIcon)
                ico = CType(Icon.FromHandle(hIcon).Clone, Icon)
                DestroyIcon(hIcon)
            End If
        End If

        Return ico
    End Function
End Class
    }
}
*/
#ifndef __XCEEDZIPAPI_H__
#define __XCEEDZIPAPI_H__

/*
 * Xceed Zip Compression Library v4.1 - API declaration
 * Copyright 1998-1999, Xceed Software Inc.
 *
 * Description:
 *    Header file for exported functions of XceedZip.dll.
 *    For use with direct access to DLL without having to instanciate the ActiveX.
 *
 * Notes on usage:
 *
 * Notes on implementation:
 *
 */

#ifdef __cplusplus
extern "C" {
#endif

////////////////////////////////////////////////////////////////////////////////
// Import declaration
//

#define XCD_IMPORT  __declspec( dllimport )
#define XCD_WINAPI  __stdcall


////////////////////////////////////////////////////////////////////////////////
// Constants
//

#define WM_USER_XCEEDZIPEVENT     ( WM_USER + 1555 )

#define XM_LISTINGFILE                 1
#define XM_PREVIEWINGFILE              2
#define XM_INSERTDISK                  3
#define XM_ZIPPREPROCESSINGFILE        4
#define XM_UNZIPPREPROCESSINGFILE      5
#define XM_SKIPPINGFILE                6
#define XM_REMOVINGFILE                7
#define XM_TESTINGFILE                 8
#define XM_FILESTATUS                  9
#define XM_GLOBALSTATUS               10
#define XM_DISKNOTEMPTY               11
#define XM_PROCESSCOMPLETED           12
#define XM_ZIPCOMMENT                 13
#define XM_QUERYMEMORYFILE            14
#define XM_ZIPPINGMEMORYFILE          15
#define XM_UNZIPPINGMEMORYFILE        16
#define XM_WARNING                    17
#define XM_INVALIDPASSWORD            18
#define XM_REPLACINGFILE              19
#define XM_ZIPCONTENTSSTATUS          20
#define XM_DELETINGFILE               21
#define XM_CONVERTPREPROCESSINGFILE   22

#define XCD_LASTDISK              0


////////////////////////////////////////////////////////////////////////////////
// Data types
//

typedef HANDLE  HXCEEDZIP;
typedef HANDLE  HXCEEDCMP;
typedef HANDLE  HXCEEDZIPITEMS;

typedef void (CALLBACK *LPFNXCEEDZIPCALLBACK)(WPARAM wXceedMessage, LPARAM lParam);


typedef enum _xcdSfxButtons
{
  xsbOk             = 0,
  xsbCancel         = 1,
  xsbAbort          = 2,
  xsbSkip           = 3,
  xsbAlwaysSkip     = 4,
  xsbYes            = 5,
  xsbNo             = 6,
  xsbOverwriteAll   = 7,
  xsbOverwriteNone  = 8,
  xsbContinue       = 9,
  xsbExit           = 10,
  xsbAgree          = 11,
  xsbRefuse         = 12
} xcdSfxButtons;

typedef enum _xcdSfxMessages
{
  xsmSuccess              = 0,
  xsmFail                 = 1,
  xsmErrorCreatingFolder  = 2,
  xsmIntro                = 3,
  xsmLicense              = 4,
  xsmDestinationFolder    = 5,
  xsmPassword             = 6,
  xsmInsertLastDisk       = 7,
  xsmInsertDisk           = 8,
  xsmAbortUnzip           = 9,
  xsmCreateFolder         = 10,
  xsmOverwrite            = 11,
  xsmProgress             = 12
} xcdSfxMessages;

typedef enum _xcdSfxStrings
{
  xssProgressBar    = 0,
  xssTitle          = 1,
  xssCurrentFolder  = 2,
  xssShareName      = 3,
  xssNetwork        = 4
} xcdSfxStrings;

typedef enum _xcdValueType
{
  xvtError          = 0,
  xvtWarning        = 1,
  xvtSkippingReason = 2
} xcdValueType;

typedef enum _xcdFileAttributes
{
  xfaNone       = 0,
  xfaReadOnly   = 1,
  xfaHidden     = 2,
  xfaSystem     = 4,
  xfaVolume     = 8,
  xfaFolder     = 16,
  xfaArchive    = 32,
  xfaCompressed = 2048
} xcdFileAttributes;

typedef enum _xcdWarning
{
  xwrIncompleteWrite            = 300,
  xwrInvalidSignature           = 301,
  xwrInvalidCentralOffset       = 302,
  xwrInvalidLocalOffset         = 303,
  xwrInvalidDescriptorOffset    = 304,
  xwrJunkInZip                  = 305,
  xwrSecurityNotSupported       = 306,
  xwrSecurityGet                = 307,
  xwrSecuritySet                = 308,
  xwrSecuritySize               = 309,
  xwrSecurityVersion            = 310,
  xwrSecurityUnknownCompression = 311,
  xwrSecurityData               = 312,
  xwrUnicodeSize                = 313,
  xwrUnicodeData                = 314,
  xwrExtraHeaderSize            = 315,
  xwrTimeStampSize              = 316,
  xwrTimeStampFlags             = 317,
  xwrFileTimesSize              = 318,
  xwrInvalidFileCount           = 319,
  xwrFileAlreadyOpenWrite       = 320,
  xwrOriginalLocSize            = 321,
  xwrOriginalLocData            = 322,
  // The following enumerations are not yet available in the ActiveX TypeLib
  xwrNoFilesMatched             = 323,
  xwrHeadersDiffer              = 324
} xcdWarning;

typedef enum _xcdError
{
  xerSuccess                = 0,
  xerProcessStarted         = 1,
  xerEmptyZipFile           = 500,
  xerSeekInZipFile          = 501,
  xerEndOfZipFile           = 502,
  xerOpenZipFile            = 503,
  xerCreateTempFile         = 504,
  xerReadZipFile            = 505,
  xerWriteTempZipFile       = 506,
  xerWriteZipFile           = 507,
  xerMoveTempFile           = 508,
  xerNothingToDo            = 509,
  xerCannotUpdateAndSpan    = 510,
  xerMemory                 = 511,
  xerSplitSizeTooSmall      = 512,
  xerSfxBinaryNotFound      = 513,
  xerReadSfxBinary          = 514,
  xerCannotUpdateSpanned    = 515,
  xerBusy                   = 516,
  xerInsertDiskAbort        = 517,
  xerUserAbort              = 518,
  xerNotAZipFile            = 519,
  xerUninitializedString    = 520,
  xerUninitializedArray     = 521,
  xerInvalidArrayDimensions = 522,
  xerInvalidArrayType       = 523,
  xerCannotAccessArray      = 524,
  xerUnsupportedDataType    = 525,
  xerWarnings               = 526,
  xerFilesSkipped           = 527,
  xerDiskNotEmptyAbort      = 528,
  xerRemoveWithoutTemp      = 529,
  xerNotLicensed            = 530,
  xerInvalidSfxProperty     = 531,
  xerInternalError          = 999
} xcdError;

typedef enum _xcdSkippingReason
{
  xsrIncluded               = 0,
  xsrFilesToExclude         = 1,
  xsrSkipExisting           = 2,
  xsrSkipNotExisting        = 3,
  xsrSkipOlderDate          = 4,
  xsrSkipOlderVersion       = 5,
  xsrRequiredAttributes     = 6,
  xsrExcludedAttributes     = 7,
  xsrMinDate                = 8,
  xsrMaxDate                = 9,
  xsrMinSize                = 10,
  xsrMaxSize                = 11,
  xsrSkipUser               = 12,
  xsrDuplicateFilenames     = 13,
  xsrSkipReplace            = 14,
  xsrUpdateWithoutTemp      = 15,
  xsrInvalidDiskNumber      = 100,
  xsrFolderWithSize         = 101,
  xsrWriteFile              = 102,
  xsrOpenFile               = 103,
  xsrReadFile               = 104,
  xsrMoveFile               = 105,
  xsrInvalidPassword        = 106,
  xsrInvalidCRC             = 107,
  xsrInvalidUncompSize      = 108,
  xsrCentralHeaderData      = 109,
  xsrLocalHeaderData        = 110,
  xsrDescriptorHeaderData   = 111,
  xsrCreateFolder           = 112,
  xsrAccessDenied           = 113,
  xsrCreateFile             = 114,
  xsrRenamedToExisting      = 116,
  xsrVolumeWithSize         = 117,
  xsrCannotSetVolumeLabel   = 118,
  xsrCentralHeaderNotFound  = 119,
  xsrUnzipDiskFull          = 120,
  xsrCompress               = 121,
  xsrUncompress             = 122
} xcdSkippingReason;

typedef enum _xcdDiskSpanning
{
  xdsNever                = 0,
  xdsAlways               = 1,
  xdsRemovableDrivesOnly  = 2
} xcdDiskSpanning;

typedef enum _xcdExtraHeader
{
  xehNone               = 0,
  xehExtTimeStamp       = 1,
  xehFileTimes          = 2,
  xehSecurityDescriptor = 4,
  xehUnicode            = 8
} xcdExtraHeader;

typedef enum _xcdSfxExistingFileBehavior
{
  xseAsk        = 0,
  xseSkip       = 1,
  xseOverwrite  = 2
} xcdSfxExistingFileBehavior;

typedef enum _xcdCurrentOperation
{
  xcoIdle               = 0,
  xcoPreviewing         = 1,
  xcoListing            = 2,
  xcoZipping            = 3,
  xcoUnzipping          = 4,
  xcoRemoving           = 5,
  xcoTestingZipFile     = 6,
  xcoGettingInformation = 7,
  xcoConverting         = 8
} xcdCurrentOperation;

typedef enum _xcdUnzipDestination
{
  xudDisk         = 0,
  xudMemory       = 1,
  xudMemoryStream = 2
} xcdUnzipDestination;

typedef enum _xcdCommonDiskSize
{
  xcsFloppy3p5HD    = 1457664,
  xcsFloppy3p5DD    = 730112,
  xcsFloppy3p5XD    = 3019898,
  xcsFloppy5p25HD   = 1310720,
  xcsFloppy5p25DD   = 655360,
  xcsIomegaZip      = 100431872,
  xcsIomegaJazz1G   = 1121976320,
  xcsIomegaJazz2G   = 2099249152,
  xcsSyquestEzFlyer = 241172480,
  xcsSyquestSyjet   = 1610612736,
  xcsSyquestSparQ   = 1073741824,
  xcsCDR            = 681574400
} xcdCommonDiskSize;

typedef enum _xcdCompressionError
{
  xceSuccess          = 0,
  xceSessionOpened    = 1000,
  xceInitCompression  = 1001,
  xceUnknownMethod    = 1002,
  xceCompression      = 1003,
  xceInvalidPassword  = 1004,
  xceChecksumFailed   = 1005,
  xceDataRemaining    = 1006,
  xceNotLicensed      = 1007,
  xceBusy             = 1008
} xcdCompressionError;

typedef enum _xcdCompressionMethod
{
  xcmStored   = 0,
  xcmDeflated = 8
} xcdCompressionMethod;

typedef enum _xcdNotEmptyAction
{
  xnaErase      = 0,
  xnaAppend     = 1,
  xnaAskAnother = 2,
  xnaAbort      = 3
} xcdNotEmptyAction;

typedef enum _xcdCompressionLevel
{
  xclNone   = 0,
  xclLow    = 1,
  xclMedium = 6,
  xclHigh   = 9
} xcdCompressionLevel;

typedef enum _xcdEvents
{
  xevNone                     = 0x00000000,
  xevListingFile              = 0x00000001,
  xevPreviewingFile           = 0x00000002,
  xevInsertDisk               = 0x00000004,
  xevZipPreprocessingFile     = 0x00000008,
  xevUnzipPreprocessingFile   = 0x00000010,
  xevSkippingFile             = 0x00000020,
  xevRemovingFile             = 0x00000040,
  xevTestingFile              = 0x00000080,
  xevFileStatus               = 0x00000100,
  xevGlobalStatus             = 0x00000200,
  xevDiskNotEmpty             = 0x00000400,
  xevProcessCompleted         = 0x00000800,
  xevZipComment               = 0x00001000,
  xevQueryMemoryFile          = 0x00002000,
  xevZippingMemoryFile        = 0x00004000,
  xevUnzippingMemoryFile      = 0x00008000,
  xevWarning                  = 0x00010000,
  xevInvalidPassword          = 0x00020000,
  xevReplacingFile            = 0x00040000,
  xevZipContentsStatus        = 0x00080000,
  xevDeletingFile             = 0x00100000,
  xevConvertPreprocessingFile = 0x00200000,
  xevAll                      = 0x7fffffff
}	xcdEvents;


////////////////////////////////////////////////////////////////////////////////
// Typedefs for all exported functions for easy GetProcAddress
//

typedef BOOL      ( XCD_WINAPI *LPFNXCEEDZIPINITDLL )( void );
typedef BOOL      ( XCD_WINAPI *LPFNXCEEDZIPSHUTDOWNDLL )( void );
typedef HXCEEDZIP ( XCD_WINAPI *LPFNXZCREATEXCEEDZIPA )( const char* );
typedef HXCEEDZIP ( XCD_WINAPI *LPFNXZCREATEXCEEDZIPW )( const WCHAR* );
typedef void      ( XCD_WINAPI *LPFNXZDESTROYXCEEDZIP )( HXCEEDZIP );
typedef HXCEEDCMP ( XCD_WINAPI *LPFNXCCREATEXCEEDCOMPRESSIONA )( const char* );
typedef HXCEEDCMP ( XCD_WINAPI *LPFNXCCREATEXCEEDCOMPRESSIONW )( const WCHAR* );
typedef void      ( XCD_WINAPI *LPFNXCDESTROYXCEEDCOMPRESSION )( HXCEEDZIP );
typedef void      ( XCD_WINAPI *LPFNXZIDESTROYXCEEDZIPITEMS )( HXCEEDZIPITEMS );
typedef void      ( XCD_WINAPI *LPFNXZSETXCEEDZIPCALLBACK )( HXCEEDZIP, LPFNXCEEDZIPCALLBACK );
typedef void      ( XCD_WINAPI *LPFNXZSETXCEEDZIPWINDOW )( HXCEEDZIP, HWND );
typedef BYTE*     ( XCD_WINAPI *LPFNXZALLOC )( DWORD );
typedef void      ( XCD_WINAPI *LPFNXZFREE )( BYTE* );

typedef BOOL  ( XCD_WINAPI *LPFNXZGETABORT )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETABORT )( HXCEEDZIP, BOOL );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETBACKGROUNDPROCESSING )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETBACKGROUNDPROCESSING )( HXCEEDZIP, BOOL );
typedef UINT  ( XCD_WINAPI *LPFNXZGETBASEPATHW )( HXCEEDZIP, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETBASEPATHA )( HXCEEDZIP, char*, UINT );
typedef void  ( XCD_WINAPI *LPFNXZSETBASEPATHW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSETBASEPATHA )( HXCEEDZIP, const char* );
typedef UINT  ( XCD_WINAPI *LPFNXZGETCOMPRESSIONLEVEL )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETCOMPRESSIONLEVEL )( HXCEEDZIP, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETCURRENTOPERATION )( HXCEEDZIP );
typedef UINT  ( XCD_WINAPI *LPFNXZGETENCRYPTIONPASSWORDW )( HXCEEDZIP, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETENCRYPTIONPASSWORDA )( HXCEEDZIP, char*,   UINT );
typedef void  ( XCD_WINAPI *LPFNXZSETENCRYPTIONPASSWORDW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSETENCRYPTIONPASSWORDA )( HXCEEDZIP, const char* );
typedef DWORD ( XCD_WINAPI *LPFNXZGETEXCLUDEDFILEATTRIBUTES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETEXCLUDEDFILEATTRIBUTES )( HXCEEDZIP, DWORD );
typedef UINT  ( XCD_WINAPI *LPFNXZGETEXTRAHEADERS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETEXTRAHEADERS )( HXCEEDZIP, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETFILESTOEXCLUDEW )( HXCEEDZIP, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETFILESTOEXCLUDEA )( HXCEEDZIP, char*, UINT );
typedef void  ( XCD_WINAPI *LPFNXZSETFILESTOEXCLUDEW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSETFILESTOEXCLUDEA )( HXCEEDZIP, const char* );
typedef UINT  ( XCD_WINAPI *LPFNXZGETFILESTOPROCESSW )( HXCEEDZIP, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETFILESTOPROCESSA )( HXCEEDZIP, char*, UINT );
typedef void  ( XCD_WINAPI *LPFNXZSETFILESTOPROCESSW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSETFILESTOPROCESSA )( HXCEEDZIP, const char* );
typedef void  ( XCD_WINAPI *LPFNXZGETMAXDATETOPROCESS )( HXCEEDZIP, LPSYSTEMTIME );
typedef void  ( XCD_WINAPI *LPFNXZSETMAXDATETOPROCESS )( HXCEEDZIP, const LPSYSTEMTIME );
typedef DWORD ( XCD_WINAPI *LPFNXZGETMAXSIZETOPROCESS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETMAXSIZETOPROCESS )( HXCEEDZIP, DWORD );
typedef void  ( XCD_WINAPI *LPFNXZGETMINDATETOPROCESS )( HXCEEDZIP, LPSYSTEMTIME );
typedef void  ( XCD_WINAPI *LPFNXZSETMINDATETOPROCESS )( HXCEEDZIP, const LPSYSTEMTIME );
typedef DWORD ( XCD_WINAPI *LPFNXZGETMINSIZETOPROCESS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETMINSIZETOPROCESS )( HXCEEDZIP, DWORD );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETPRESERVEPATHS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETPRESERVEPATHS )( HXCEEDZIP, BOOL );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETPROCESSSUBFOLDERS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETPROCESSSUBFOLDERS )( HXCEEDZIP, BOOL );
typedef DWORD ( XCD_WINAPI *LPFNXZGETREQUIREDFILEATTRIBUTES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETREQUIREDFILEATTRIBUTES )( HXCEEDZIP, DWORD );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXBINARYMODULEW )( HXCEEDZIP, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXBINARYMODULEA )( HXCEEDZIP, char*, UINT );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXBINARYMODULEW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXBINARYMODULEA )( HXCEEDZIP, const char* );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXBUTTONSW )( HXCEEDZIP, xcdSfxButtons xIndex, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXBUTTONSA )( HXCEEDZIP, xcdSfxButtons xIndex, char* pszBuffer, UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXBUTTONSW )( HXCEEDZIP, xcdSfxButtons xIndex, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXBUTTONSA )( HXCEEDZIP, xcdSfxButtons xIndex, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXDEFAULTPASSWORDW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXDEFAULTPASSWORDA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXDEFAULTPASSWORDW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXDEFAULTPASSWORDA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXDEFAULTUNZIPTOFOLDERW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXDEFAULTUNZIPTOFOLDERA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXDEFAULTUNZIPTOFOLDERW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXDEFAULTUNZIPTOFOLDERA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXEXISTINGFILEBEHAVIOR )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXEXISTINGFILEBEHAVIOR )( HXCEEDZIP, UINT uValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXEXTENSIONSTOASSOCIATEW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXEXTENSIONSTOASSOCIATEA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXEXTENSIONSTOASSOCIATEW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXEXTENSIONSTOASSOCIATEA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXEXECUTEAFTERW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXEXECUTEAFTERA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXEXECUTEAFTERW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXEXECUTEAFTERA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXICONFILENAMEW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXICONFILENAMEA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXICONFILENAMEW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXICONFILENAMEA )( HXCEEDZIP, const char* pszValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETSFXINSTALLMODE )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXINSTALLMODE )( HXCEEDZIP, BOOL bValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXMESSAGESW )( HXCEEDZIP, xcdSfxMessages xIndex, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXMESSAGESA )( HXCEEDZIP, xcdSfxMessages xIndex, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXMESSAGESW )( HXCEEDZIP, xcdSfxMessages xIndex, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXMESSAGESA )( HXCEEDZIP, xcdSfxMessages xIndex, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXPROGRAMGROUPW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXPROGRAMGROUPA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXPROGRAMGROUPW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXPROGRAMGROUPA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXPROGRAMGROUPITEMSW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXPROGRAMGROUPITEMSA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXPROGRAMGROUPITEMSW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXPROGRAMGROUPITEMSA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXREADMEFILEW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXREADMEFILEA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXREADMEFILEW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXREADMEFILEA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXSTRINGSW )( HXCEEDZIP, xcdSfxStrings xIndex, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXSTRINGSA )( HXCEEDZIP, xcdSfxStrings xIndex, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXSTRINGSW )( HXCEEDZIP, xcdSfxStrings xIndex, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXSTRINGSA )( HXCEEDZIP, xcdSfxStrings xIndex, const char* pszValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETSKIPIFEXISTING )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSKIPIFEXISTING )( HXCEEDZIP, BOOL bValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETSKIPIFNOTEXISTING )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSKIPIFNOTEXISTING )( HXCEEDZIP, BOOL bValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETSKIPIFOLDERDATE )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSKIPIFOLDERDATE )( HXCEEDZIP, BOOL bValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETSKIPIFOLDERVERSION )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSKIPIFOLDERVERSION )( HXCEEDZIP, BOOL bValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSPANMULTIPLEDISKS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSPANMULTIPLEDISKS )( HXCEEDZIP, UINT uValue );
typedef DWORD ( XCD_WINAPI *LPFNXZGETSPLITSIZE )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETSPLITSIZE )( HXCEEDZIP, DWORD dwValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETTEMPFOLDERW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETTEMPFOLDERA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETTEMPFOLDERW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETTEMPFOLDERA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETUNZIPTOFOLDERW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETUNZIPTOFOLDERA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETUNZIPTOFOLDERW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETUNZIPTOFOLDERA )( HXCEEDZIP, const char* pszValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETUSETEMPFILE )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETUSETEMPFILE )( HXCEEDZIP, BOOL bValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETZIPFILENAMEW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETZIPFILENAMEA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETZIPFILENAMEW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETZIPFILENAMEA )( HXCEEDZIP, const char* pszValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETZIPOPENEDFILES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETZIPOPENEDFILES )( HXCEEDZIP, BOOL bValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXFILESTOCOPYW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXFILESTOCOPYA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXFILESTOCOPYW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXFILESTOCOPYA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXFILESTOREGISTERW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXFILESTOREGISTERA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXFILESTOREGISTERW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXFILESTOREGISTERA )( HXCEEDZIP, const char* pszValue );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXREGISTRYKEYSW )( HXCEEDZIP, WCHAR* pwszBuffer, UINT uMaxLength );
typedef UINT  ( XCD_WINAPI *LPFNXZGETSFXREGISTRYKEYSA )( HXCEEDZIP, char* pszBuffer,   UINT uMaxLength );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXREGISTRYKEYSW )( HXCEEDZIP, const WCHAR* pwszValue );
typedef void  ( XCD_WINAPI *LPFNXZSETSFXREGISTRYKEYSA )( HXCEEDZIP, const char* pszValue );
typedef BOOL  ( XCD_WINAPI *LPFNXZGETDELETEZIPPEDFILES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETDELETEZIPPEDFILES )( HXCEEDZIP, BOOL );
typedef DWORD ( XCD_WINAPI *LPFNXZGETFIRSTDISKFREESPACE )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETFIRSTDISKFREESPACE )( HXCEEDZIP, DWORD dwValue );
typedef DWORD ( XCD_WINAPI *LPFNXZGETMINDISKFREESPACE )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETMINDISKFREESPACE )( HXCEEDZIP, DWORD dwValue );
typedef DWORD ( XCD_WINAPI *LPFNXZGETEVENTSTOTRIGGER )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSETEVENTSTOTRIGGER )( HXCEEDZIP, DWORD dwValue );

typedef UINT  ( XCD_WINAPI *LPFNXCGETENCRYPTIONPASSWORDW )( HXCEEDCMP, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXCGETENCRYPTIONPASSWORDA )( HXCEEDCMP, char*, UINT );
typedef void  ( XCD_WINAPI *LPFNXCSETENCRYPTIONPASSWORDW )( HXCEEDCMP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXCSETENCRYPTIONPASSWORDA )( HXCEEDCMP, const char* );
typedef int   ( XCD_WINAPI *LPFNXCGETCOMPRESSIONLEVEL )( HXCEEDCMP );
typedef void  ( XCD_WINAPI *LPFNXCSETCOMPRESSIONLEVEL )( HXCEEDCMP, int );

typedef void  ( XCD_WINAPI *LPFNXZADDFILESTOPROCESSW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZADDFILESTOPROCESSA )( HXCEEDZIP, const char* );
typedef void  ( XCD_WINAPI *LPFNXZADDFILESTOEXCLUDEW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZADDFILESTOEXCLUDEA )( HXCEEDZIP, const char* );
typedef int   ( XCD_WINAPI *LPFNXZCONVERTW )( HXCEEDZIP, const WCHAR* );
typedef int   ( XCD_WINAPI *LPFNXZCONVERTA )( HXCEEDZIP, const char* );
typedef UINT  ( XCD_WINAPI *LPFNXZGETERRORDESCRIPTIONW )( HXCEEDZIP, xcdValueType, int, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXZGETERRORDESCRIPTIONA )( HXCEEDZIP, xcdValueType, int, char*,   UINT );
typedef int   ( XCD_WINAPI *LPFNXZGETZIPFILEINFORMATION )( HXCEEDZIP, LONG*, LONG*, LONG*, SHORT*, BOOL* );
typedef int   ( XCD_WINAPI *LPFNXZLISTZIPCONTENTS )( HXCEEDZIP );
typedef int   ( XCD_WINAPI *LPFNXZPREVIEWFILES )( HXCEEDZIP, BOOL );
typedef int   ( XCD_WINAPI *LPFNXZREMOVEFILES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDEXTENSIONTOASSOCIATEW )( HXCEEDZIP, const WCHAR*, const WCHAR*, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDEXTENSIONTOASSOCIATEA )( HXCEEDZIP, const char*,   const char*,   const char* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDPROGRAMGROUPITEMW )( HXCEEDZIP, const WCHAR*, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDPROGRAMGROUPITEMA )( HXCEEDZIP, const char*,   const char* );
typedef void  ( XCD_WINAPI *LPFNXZSFXCLEARBUTTONS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSFXCLEARMESSAGES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSFXCLEARSTRINGS )( HXCEEDZIP );
typedef BOOL  ( XCD_WINAPI *LPFNXZSFXLOADCONFIGW )( HXCEEDZIP, const WCHAR* );
typedef BOOL  ( XCD_WINAPI *LPFNXZSFXLOADCONFIGA )( HXCEEDZIP, const char* );
typedef BOOL  ( XCD_WINAPI *LPFNXZSFXSAVECONFIGW )( HXCEEDZIP, const WCHAR* );
typedef BOOL  ( XCD_WINAPI *LPFNXZSFXSAVECONFIGA )( HXCEEDZIP, const char* );
typedef void  ( XCD_WINAPI *LPFNXZSFXRESETBUTTONS )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSFXRESETMESSAGES )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSFXRESETSTRINGS )( HXCEEDZIP );
typedef int   ( XCD_WINAPI *LPFNXZTESTZIPFILE )( HXCEEDZIP, BOOL );
typedef int   ( XCD_WINAPI *LPFNXZUNZIP )( HXCEEDZIP );
typedef int   ( XCD_WINAPI *LPFNXZZIP )( HXCEEDZIP );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDEXECUTEAFTERW )( HXCEEDZIP, const WCHAR*, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDEXECUTEAFTERA )( HXCEEDZIP, const char*,  const char* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDFILETOCOPYW )( HXCEEDZIP, const WCHAR*, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDFILETOCOPYA )( HXCEEDZIP, const char*,  const char* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDFILETOREGISTERW )( HXCEEDZIP, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDFILETOREGISTERA )( HXCEEDZIP, const char* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDREGISTRYKEYW )( HXCEEDZIP, const WCHAR*, const WCHAR*, const WCHAR* );
typedef void  ( XCD_WINAPI *LPFNXZSFXADDREGISTRYKEYA )( HXCEEDZIP, const char*,  const char*,  const char* );
typedef int   ( XCD_WINAPI *LPFNXZGETZIPCONTENTS )( HXCEEDZIP, HXCEEDZIPITEMS* );

typedef UINT  ( XCD_WINAPI *LPFNXCGETERRORDESCRIPTIONW )( HXCEEDCMP, int, WCHAR*, UINT );
typedef UINT  ( XCD_WINAPI *LPFNXCGETERRORDESCRIPTIONA )( HXCEEDCMP, int, char*, UINT );
typedef int   ( XCD_WINAPI *LPFNXCCOMPRESS )( HXCEEDCMP, const BYTE*, DWORD, BYTE**, DWORD*, BOOL );
typedef int   ( XCD_WINAPI *LPFNXCUNCOMPRESS )( HXCEEDCMP, const BYTE*, DWORD, BYTE**, DWORD*, BOOL );
typedef long  ( XCD_WINAPI *LPFNXCCALCULATECRC )( HXCEEDCMP, const BYTE*, DWORD, long );


////////////////////////////////////////////////////////////////////////////////
// Structures for callbacks / messages params
//

// ListingFile event parameters
typedef struct _xcdListingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ char                  szFilename[ MAX_PATH ];
  /* IN  */ char                  szComment[ 1024 ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ SHORT                 nCompressionRatio;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stLastModified;
  /* IN  */ SYSTEMTIME            stLastAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ LONG                  lDiskNumber;
  /* IN  */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
} xcdListingFileParamsA;

typedef struct _xcdListingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ WCHAR                 szFilename[ MAX_PATH ];
  /* IN  */ WCHAR                 szComment[ 1024 ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ SHORT                 nCompressionRatio;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stLastModified;
  /* IN  */ SYSTEMTIME            stLastAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ LONG                  lDiskNumber;
  /* IN  */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
} xcdListingFileParamsW;

// PreviewingFile event parameters
typedef struct _xcdPreviewingFileParamsA
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ char              szFilename[ MAX_PATH ];
  /* IN  */ char              szSourceFilename[ MAX_PATH ];
  /* IN  */ LONG              lSize;
  /* IN  */ xcdFileAttributes xAttributes;
  /* IN  */ SYSTEMTIME        stModified;
  /* IN  */ SYSTEMTIME        stAccessed;
  /* IN  */ SYSTEMTIME        stCreated;
  /* IN  */ BOOL              bExcluded;
  /* IN  */ xcdSkippingReason xReason;
} xcdPreviewingFileParamsA;

typedef struct _xcdPreviewingFileParamsW
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ WCHAR             szFilename[ MAX_PATH ];
  /* IN  */ WCHAR             szSourceFilename[ MAX_PATH ];
  /* IN  */ LONG              lSize;
  /* IN  */ xcdFileAttributes xAttributes;
  /* IN  */ SYSTEMTIME        stModified;
  /* IN  */ SYSTEMTIME        stAccessed;
  /* IN  */ SYSTEMTIME        stCreated;
  /* IN  */ BOOL              bExcluded;
  /* IN  */ xcdSkippingReason xReason;
} xcdPreviewingFileParamsW;

// InsertDisk event parameters
typedef struct _xcdInsertDiskParams
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ LONG            lDiskNumber;
  /* OUT */ BOOL            bDiskInserted;
} xcdInsertDiskParams;

// ZipPreprocessingFile event parameters
typedef struct _xcdZipPreprocessingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* OUT */ char                  szFilename[ MAX_PATH ];
  /* OUT */ char                  szComment[ 1024 ];
  /* IN  */ char                  szSourceFilename[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* OUT */ xcdFileAttributes     xAttributes;
  /* OUT */ SYSTEMTIME            stModified;
  /* OUT */ SYSTEMTIME            stAccessed;
  /* OUT */ SYSTEMTIME            stCreated;
  /* OUT */ xcdCompressionMethod  xMethod;
  /* OUT */ BOOL                  bEncrypted;
  /* OUT */ char                  szPassword[ MAX_PATH ];
  /* OUT */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
  /* IN  */ BOOL                  bExisting;
} xcdZipPreprocessingFileParamsA;

typedef struct _xcdZipPreprocessingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* OUT */ WCHAR                 szFilename[ MAX_PATH ];
  /* OUT */ WCHAR                 szComment[ 1024 ];
  /* IN  */ WCHAR                 szSourceFilename[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* OUT */ xcdFileAttributes     xAttributes;
  /* OUT */ SYSTEMTIME            stModified;
  /* OUT */ SYSTEMTIME            stAccessed;
  /* OUT */ SYSTEMTIME            stCreated;
  /* OUT */ xcdCompressionMethod  xMethod;
  /* OUT */ BOOL                  bEncrypted;
  /* OUT */ WCHAR                 szPassword[ MAX_PATH ];
  /* OUT */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
  /* IN  */ BOOL                  bExisting;
} xcdZipPreprocessingFileParamsW;

// UnzipPreprocessingFile event parameters
typedef struct _xcdUnzipPreprocessingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ char                  szFilename[ MAX_PATH ];
  /* IN  */ char                  szComment[ 1024 ];
  /* OUT */ char                  szDestFilename[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* OUT */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* OUT */ SYSTEMTIME            stModified;
  /* OUT */ SYSTEMTIME            stAccessed;
  /* OUT */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* OUT */ char                  szPassword[ MAX_PATH ];
  /* IN  */ LONG                  lDiskNumber;
  /* OUT */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
  /* IN  */ BOOL                  bExisting;
  /* OUT */ xcdUnzipDestination   xDestination;
} xcdUnzipPreprocessingFileParamsA;

typedef struct _xcdUnzipPreprocessingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ WCHAR                 szFilename[ MAX_PATH ];
  /* IN  */ WCHAR                 szComment[ 1024 ];
  /* OUT */ WCHAR                 szDestFilename[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* OUT */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* OUT */ SYSTEMTIME            stModified;
  /* OUT */ SYSTEMTIME            stAccessed;
  /* OUT */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* OUT */ WCHAR                 szPassword[ MAX_PATH ];
  /* IN  */ LONG                  lDiskNumber;
  /* OUT */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
  /* IN  */ BOOL                  bExisting;
  /* OUT */ xcdUnzipDestination   xDestination;
} xcdUnzipPreprocessingFileParamsW;

// SkippingFile event parameters
typedef struct _xcdSkippingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ char                  szFilename[ MAX_PATH ];
  /* IN  */ char                  szComment[ 1024 ];
  /* IN  */ char                  szFilenameOnDisk[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stModified;
  /* IN  */ SYSTEMTIME            stAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ xcdSkippingReason     xReason;
} xcdSkippingFileParamsA;

typedef struct _xcdSkippingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ WCHAR                 szFilename[ MAX_PATH ];
  /* IN  */ WCHAR                 szComment[ 1024 ];
  /* IN  */ WCHAR                 szFilenameOnDisk[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stModified;
  /* IN  */ SYSTEMTIME            stAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ xcdSkippingReason     xReason;
} xcdSkippingFileParamsW;

// RemovingFile event parameters
typedef struct _xcdRemovingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ char                  szFilename[ MAX_PATH ];
  /* IN  */ char                  szComment[ 1024 ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stModified;
  /* IN  */ SYSTEMTIME            stAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
} xcdRemovingFileParamsA;

typedef struct _xcdRemovingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ WCHAR                 szFilename[ MAX_PATH ];
  /* IN  */ WCHAR                 szComment[ 1024 ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stModified;
  /* IN  */ SYSTEMTIME            stAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
} xcdRemovingFileParamsW;

// TestingFile event parameters
typedef struct _xcdTestingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ char                  szFilename[ MAX_PATH ];
  /* IN  */ char                  szComment[ 1024 ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ SHORT                 nCompressionRatio;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stModified;
  /* IN  */ SYSTEMTIME            stAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ LONG                  lDiskNumber;
} xcdTestingFileParamsA;

typedef struct _xcdTestingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ WCHAR                 szFilename[ MAX_PATH ];
  /* IN  */ WCHAR                 szComment[ 1024 ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* IN  */ SHORT                 nCompressionRatio;
  /* IN  */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* IN  */ SYSTEMTIME            stModified;
  /* IN  */ SYSTEMTIME            stAccessed;
  /* IN  */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ LONG                  lDiskNumber;
} xcdTestingFileParamsW;

// FileStatus event parameters
typedef struct _xcdFileStatusParamsA
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ char            szFilename[ MAX_PATH ];
  /* IN  */ LONG            lSize;
  /* IN  */ LONG            lCompressedSize;
  /* IN  */ LONG            lBytesProcessed;
  /* IN  */ SHORT           nBytesPercent;
  /* IN  */ SHORT           nCompressionRatio;
  /* IN  */ BOOL            bFileCompleted;
} xcdFileStatusParamsA;

typedef struct _xcdFileStatusParamsW
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ WCHAR           szFilename[ MAX_PATH ];
  /* IN  */ LONG            lSize;
  /* IN  */ LONG            lCompressedSize;
  /* IN  */ LONG            lBytesProcessed;
  /* IN  */ SHORT           nBytesPercent;
  /* IN  */ SHORT           nCompressionRatio;
  /* IN  */ BOOL            bFileCompleted;
} xcdFileStatusParamsW;

// GlobalStatus event parameters
typedef struct _xcdGlobalStatusParams
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ LONG            lFilesTotal;
  /* IN  */ LONG            lFilesProcessed;
  /* IN  */ LONG            lFilesSkipped;
  /* IN  */ SHORT           nFilesPercent;
  /* IN  */ LONG            lBytesTotal;
  /* IN  */ LONG            lBytesProcessed;
  /* IN  */ LONG            lBytesSkipped;
  /* IN  */ SHORT           nBytesPercent;
  /* IN  */ LONG            lBytesOutput;
  /* IN  */ SHORT           nCompressionRatio;
} xcdGlobalStatusParams;

// DiskNotEmpty event parameters
typedef struct _xcdDiskNotEmptyParams
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* OUT */ xcdNotEmptyAction xAction;
} xcdDiskNotEmptyParams;

// ProcessCompleted event parameters
typedef struct _xcdProcessCompletedParams
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ LONG            lFilesTotal;
  /* IN  */ LONG            lFilesProcessed;
  /* IN  */ LONG            lFilesSkipped;
  /* IN  */ LONG            lBytesTotal;
  /* IN  */ LONG            lBytesProcessed;
  /* IN  */ LONG            lBytesSkipped;
  /* IN  */ LONG            lBytesOutput;
  /* IN  */ SHORT           nCompressionRatio;
  /* IN  */ xcdError        xResult;
} xcdProcessCompletedParams;

// ZipComment event parameters
typedef struct _xcdZipCommentParamsA
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* OUT */ char            szComment[ 10240 ];
} xcdZipCommentParamsA;

typedef struct _xcdZipCommentParamsW
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* OUT */ WCHAR           szComment[ 10240 ];
} xcdZipCommentParamsW;

// QueryMemoryFile event parameters
typedef struct _xcdQueryMemoryFileParamsA
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* OUT */ LONG              lUserTag;
  /* OUT */ char              szFilename[ MAX_PATH ];
  /* OUT */ char              szComment[ 1024 ];
  /* OUT */ xcdFileAttributes xAttributes;
  /* OUT */ SYSTEMTIME        stModified;
  /* OUT */ SYSTEMTIME        stAccessed;
  /* OUT */ SYSTEMTIME        stCreated;
  /* OUT */ BOOL              bEncrypted;
  /* OUT */ char              szPassword[ MAX_PATH ];
  /* OUT */ BOOL              bFileProvided;
} xcdQueryMemoryFileParamsA;

typedef struct _xcdQueryMemoryFileParamsW
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* OUT */ LONG              lUserTag;
  /* OUT */ WCHAR             szFilename[ MAX_PATH ];
  /* OUT */ WCHAR             szComment[ 1024 ];
  /* OUT */ xcdFileAttributes xAttributes;
  /* OUT */ SYSTEMTIME        stModified;
  /* OUT */ SYSTEMTIME        stAccessed;
  /* OUT */ SYSTEMTIME        stCreated;
  /* OUT */ BOOL              bEncrypted;
  /* OUT */ WCHAR             szPassword[ MAX_PATH ];
  /* OUT */ BOOL              bFileProvided;
} xcdQueryMemoryFileParamsW;

// ZippingMemoryFile event parameters
typedef struct _xcdZippingMemoryFileParamsA
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ LONG            lUserTag;
  /* IN  */ char            szFilename[ MAX_PATH ];
  /* OUT */ BYTE *          pbDataToCompress;
  /* OUT */ DWORD           dwDataSize;
  /* OUT */ BOOL            bEndOfData;
} xcdZippingMemoryFileParamsA;

typedef struct _xcdZippingMemoryFileParamsW
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ LONG            lUserTag;
  /* IN  */ WCHAR           szFilename[ MAX_PATH ];
  /* OUT */ BYTE *          pbDataToCompress;
  /* OUT */ DWORD           dwDataSize;
  /* OUT */ BOOL            bEndOfData;
} xcdZippingMemoryFileParamsW;

// UnzippingMemoryFile event parameters
typedef struct _xcdUnzippingMemoryFileParamsA
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ char            szFilename[ MAX_PATH ];
  /* IN  */ BYTE *          pbUncompressedData;
  /* IN  */ DWORD           dwDataSize;
  /* IN  */ BOOL            bEndOfData;
} xcdUnzippingMemoryFileParamsA;

typedef struct _xcdUnzippingMemoryFileParamsW
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ WCHAR           szFilename[ MAX_PATH ];
  /* IN  */ BYTE *          pbUncompressedData;
  /* IN  */ DWORD           dwDataSize;
  /* IN  */ BOOL            bEndOfData;
} xcdUnzippingMemoryFileParamsW;

// Warnign event parameters
typedef struct _xcdWarningParamsA
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ char              szFilename[ MAX_PATH ];
  /* IN  */ xcdWarning        xWarning;
} xcdWarningParamsA;

typedef struct _xcdWarningParamsW
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ WCHAR             szFilename[ MAX_PATH ];
  /* IN  */ xcdWarning        xWarning;
} xcdWarningParamsW;

// InvalidPassword event parameters
typedef struct _xcdInvalidPasswordParamsA
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ char            szFilename[ MAX_PATH ];
  /* OUT */ char            szNewPassword[ MAX_PATH ];
  /* OUT */ BOOL            bRetry;
} xcdInvalidPasswordParamsA;

typedef struct _xcdInvalidPasswordParamsW
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ WCHAR           szFilename[ MAX_PATH ];
  /* OUT */ WCHAR           szNewPassword[ MAX_PATH ];
  /* OUT */ BOOL            bRetry;
} xcdInvalidPasswordParamsW;

// ReplacingFile event parameters
typedef struct _xcdReplacingFileParamsA
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ char              szFilename[ MAX_PATH ];
  /* IN  */ char              szComment[ 1024 ];
  /* IN  */ LONG              lSize;
  /* IN  */ xcdFileAttributes xAttributes;
  /* IN  */ SYSTEMTIME        stLastModified;
  /* IN  */ SYSTEMTIME        stLastAccessed;
  /* IN  */ SYSTEMTIME        stCreated;
  /* IN  */ char              szOrigFilename[ MAX_PATH ];
  /* IN  */ LONG              lOrigSize;
  /* IN  */ xcdFileAttributes xOrigAttributes;
  /* IN  */ SYSTEMTIME        stOrigLastModified;
  /* IN  */ SYSTEMTIME        stOrigLastAccessed;
  /* IN  */ SYSTEMTIME        stOrigCreated;
  /* OUT */ BOOL              bReplaceFile;
} xcdReplacingFileParamsA;

typedef struct _xcdReplacingFileParamsW
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ WCHAR             szFilename[ MAX_PATH ];
  /* IN  */ WCHAR             szComment[ 1024 ];
  /* IN  */ LONG              lSize;
  /* IN  */ xcdFileAttributes xAttributes;
  /* IN  */ SYSTEMTIME        stLastModified;
  /* IN  */ SYSTEMTIME        stLastAccessed;
  /* IN  */ SYSTEMTIME        stCreated;
  /* IN  */ WCHAR             szOrigFilename[ MAX_PATH ];
  /* IN  */ LONG              lOrigSize;
  /* IN  */ xcdFileAttributes xOrigAttributes;
  /* IN  */ SYSTEMTIME        stOrigLastModified;
  /* IN  */ SYSTEMTIME        stOrigLastAccessed;
  /* IN  */ SYSTEMTIME        stOrigCreated;
  /* OUT */ BOOL              bReplaceFile;
} xcdReplacingFileParamsW;

// ZipContentsStatus event parameters
typedef struct _xcdZipContentsStatusParams
{
  /* IN  */ WORD            wStructSize;
  /* IN  */ HXCEEDZIP       hZip;
  /* IN  */ LONG            lFilesTotal;
  /* IN  */ LONG            lFilesRead;
  /* IN  */ SHORT           nFilesPercent;
} xcdZipContentsStatusParams;

// DeletingFile event parameters
typedef struct _xcdDeletingFileParamsA
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ char              szFilename[ MAX_PATH ];
  /* IN  */ LONG              lSize;
  /* IN  */ xcdFileAttributes xAttributes;
  /* IN  */ SYSTEMTIME        stModified;
  /* IN  */ SYSTEMTIME        stAccessed;
  /* IN  */ SYSTEMTIME        stCreated;
  /* OUT */ BOOL              bDoNotDelete;
} xcdDeletingFileParamsA;

typedef struct _xcdDeletingFileParamsW
{
  /* IN  */ WORD              wStructSize;
  /* IN  */ HXCEEDZIP         hZip;
  /* IN  */ WCHAR             wszFilename[ MAX_PATH ];
  /* IN  */ LONG              lSize;
  /* IN  */ xcdFileAttributes xAttributes;
  /* IN  */ SYSTEMTIME        stModified;
  /* IN  */ SYSTEMTIME        stAccessed;
  /* IN  */ SYSTEMTIME        stCreated;
  /* OUT */ BOOL              bDoNotDelete;
} xcdDeletingFileParamsW;

// ConvertPreprocessingFile event parameters
typedef struct _xcdConvertPreprocessingFileParamsA
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ char                  szFilename[ MAX_PATH ];
  /* OUT */ char                  szComment[ 1024 ];
  /* OUT */ char                  szDestFilename[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* OUT */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* OUT */ SYSTEMTIME            stModified;
  /* OUT */ SYSTEMTIME            stAccessed;
  /* OUT */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ LONG                  lDiskNumber;
  /* OUT */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
  /* IN  */ BOOL                  bExisting;
} xcdConvertPreprocessingFileParamsA;

typedef struct _xcdConvertPreprocessingFileParamsW
{
  /* IN  */ WORD                  wStructSize;
  /* IN  */ HXCEEDZIP             hZip;
  /* IN  */ WCHAR                 wszFilename[ MAX_PATH ];
  /* OUT */ WCHAR                 wszComment[ 1024 ];
  /* OUT */ WCHAR                 wszDestFilename[ MAX_PATH ];
  /* IN  */ LONG                  lSize;
  /* IN  */ LONG                  lCompressedSize;
  /* OUT */ xcdFileAttributes     xAttributes;
  /* IN  */ LONG                  lCRC;
  /* OUT */ SYSTEMTIME            stModified;
  /* OUT */ SYSTEMTIME            stAccessed;
  /* OUT */ SYSTEMTIME            stCreated;
  /* IN  */ xcdCompressionMethod  xMethod;
  /* IN  */ BOOL                  bEncrypted;
  /* IN  */ LONG                  lDiskNumber;
  /* OUT */ BOOL                  bExcluded;
  /* IN  */ xcdSkippingReason     xReason;
  /* IN  */ BOOL                  bExisting;
} xcdConvertPreprocessingFileParamsW;


//
// More typedefs for exported functions that use event structures
//

typedef BOOL  ( XCD_WINAPI *LPFNXZIGETFIRSTITEMW )( HXCEEDZIPITEMS, xcdListingFileParamsW* );
typedef BOOL  ( XCD_WINAPI *LPFNXZIGETFIRSTITEMA )( HXCEEDZIPITEMS, xcdListingFileParamsA* );
typedef BOOL  ( XCD_WINAPI *LPFNXZIGETNEXTITEMW )( HXCEEDZIPITEMS, xcdListingFileParamsW* );
typedef BOOL  ( XCD_WINAPI *LPFNXZIGETNEXTITEMA )( HXCEEDZIPITEMS, xcdListingFileParamsA* );


////////////////////////////////////////////////////////////////////////////////
// Exported prototypes for use when LINKING with XCEEDZIP.LIB
// (only in Visual C++)
//

#define _MSC_VER

#ifdef _MSC_VER

//
// Initialization functions
//
XCD_IMPORT  BOOL  XCD_WINAPI  XceedZipInitDLL( void );
XCD_IMPORT  BOOL  XCD_WINAPI  XceedZipShutdownDLL( void );

//
// Instance creation functions
//
XCD_IMPORT  HXCEEDZIP XCD_WINAPI  XzCreateXceedZipA( const char* pszLicense );
XCD_IMPORT  HXCEEDZIP XCD_WINAPI  XzCreateXceedZipW( const WCHAR* pwszLicense );
XCD_IMPORT  void      XCD_WINAPI  XzDestroyXceedZip( HXCEEDZIP hZip );

XCD_IMPORT  HXCEEDCMP XCD_WINAPI  XcCreateXceedCompressionA( const char* pszLicense );
XCD_IMPORT  HXCEEDCMP XCD_WINAPI  XcCreateXceedCompressionW( const WCHAR* pwszLicense );
XCD_IMPORT  void      XCD_WINAPI  XcDestroyXceedCompression( HXCEEDCMP hComp );

XCD_IMPORT  void      XCD_WINAPI  XziDestroyXceedZipItems( HXCEEDZIPITEMS hItems );

//
// Event handling functions
//
XCD_IMPORT  void      XCD_WINAPI  XzSetXceedZipCallback( HXCEEDZIP hZip, LPFNXCEEDZIPCALLBACK lpfnCallback );
XCD_IMPORT  void      XCD_WINAPI  XzSetXceedZipWindow( HXCEEDZIP hZip, HWND hWnd );

//
// Memory allocation functions
//
XCD_IMPORT  BYTE*     XCD_WINAPI  XzAlloc( DWORD dwDataSize );
XCD_IMPORT  void      XCD_WINAPI  XzFree( BYTE* pcBuffer );


//
// XceedZip property handling functions
//

// Abort property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetAbort( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetAbort( HXCEEDZIP hZip, BOOL bValue );

// Background property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetBackgroundProcessing( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetBackgroundProcessing( HXCEEDZIP hZip, BOOL bValue );

// BasePath property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetBasePathW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetBasePathA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetBasePathW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetBasePathA( HXCEEDZIP hZip, const char* pszValue );

// CompressionLevel property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetCompressionLevel( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetCompressionLevel( HXCEEDZIP hZip, UINT uValue );

// CurrentOperation property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetCurrentOperation( HXCEEDZIP hZip );

// EncryptionPassword property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetEncryptionPasswordW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetEncryptionPasswordA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetEncryptionPasswordW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetEncryptionPasswordA( HXCEEDZIP hZip, const char* pwszValue );

// ExcludedFileAttributes property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetExcludedFileAttributes( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetExcludedFileAttributes( HXCEEDZIP hZip, DWORD dwValue );

// ExtraHeaders property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetExtraHeaders( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetExtraHeaders( HXCEEDZIP hZip, UINT uValue );

// FilesToExclude property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetFilesToExcludeW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetFilesToExcludeA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetFilesToExcludeW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetFilesToExcludeA( HXCEEDZIP hZip, const char* pszValue );

// FilesToProcess property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetFilesToProcessW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetFilesToProcessA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetFilesToProcessW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetFilesToProcessA( HXCEEDZIP hZip, const char* pszValue );

// MaxDateToProcess property
XCD_IMPORT  void  XCD_WINAPI  XzGetMaxDateToProcess( HXCEEDZIP hZip, LPSYSTEMTIME lpdtBuffer );
XCD_IMPORT  void  XCD_WINAPI  XzSetMaxDateToProcess( HXCEEDZIP hZip, const LPSYSTEMTIME lpdtValue );

// MaxSizeToProcess property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetMaxSizeToProcess( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetMaxSizeToProcess( HXCEEDZIP hZip, DWORD dwValue );

// MinDateToProcess property
XCD_IMPORT  void  XCD_WINAPI  XzGetMinDateToProcess( HXCEEDZIP hZip, LPSYSTEMTIME lpdtBuffer );
XCD_IMPORT  void  XCD_WINAPI  XzSetMinDateToProcess( HXCEEDZIP hZip, const LPSYSTEMTIME lpdtValue );

// MinSizeToProcess property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetMinSizeToProcess( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetMinSizeToProcess( HXCEEDZIP hZip, DWORD dwValue );

// PreservePaths property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetPreservePaths( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetPreservePaths( HXCEEDZIP hZip, BOOL bValue );

// ProcessSubfolders property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetProcessSubfolders( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetProcessSubfolders( HXCEEDZIP hZip, BOOL bValue );

// RequiredFileAttributes property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetRequiredFileAttributes( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetRequiredFileAttributes( HXCEEDZIP hZip, DWORD dwValue );

// SfxBinaryModule property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxBinaryModuleW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxBinaryModuleA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxBinaryModuleW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxBinaryModuleA( HXCEEDZIP hZip, const char* pszValue );

// SfxButtons property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxButtonsW( HXCEEDZIP hZip, xcdSfxButtons xIndex, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxButtonsA( HXCEEDZIP hZip, xcdSfxButtons xIndex, char* pszBuffer, UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxButtonsW( HXCEEDZIP hZip, xcdSfxButtons xIndex, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxButtonsA( HXCEEDZIP hZip, xcdSfxButtons xIndex, const char* pszValue );

// SfxDefaultPassword property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxDefaultPasswordW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxDefaultPasswordA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxDefaultPasswordW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxDefaultPasswordA( HXCEEDZIP hZip, const char* pszValue );

// SfxDefaultUnzipToFolder property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxDefaultUnzipToFolderW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxDefaultUnzipToFolderA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxDefaultUnzipToFolderW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxDefaultUnzipToFolderA( HXCEEDZIP hZip, const char* pszValue );

// SfxExistingFileBehavior property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxExistingFileBehavior( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxExistingFileBehavior( HXCEEDZIP hZip, UINT uValue );

// SfxExtensionsToAssociate property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxExtensionsToAssociateW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxExtensionsToAssociateA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxExtensionsToAssociateW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxExtensionsToAssociateA( HXCEEDZIP hZip, const char* pszValue );

// SfxExecuteAfter property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxExecuteAfterW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxExecuteAfterA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxExecuteAfterW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxExecuteAfterA( HXCEEDZIP hZip, const char* pszValue );

// SfxIconFilename property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxIconFilenameW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxIconFilenameA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxIconFilenameW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxIconFilenameA( HXCEEDZIP hZip, const char* pszValue );

// SfxInstallMode property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetSfxInstallMode( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxInstallMode( HXCEEDZIP hZip, BOOL bValue );

// SfxMessages property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxMessagesW( HXCEEDZIP hZip, xcdSfxMessages xIndex, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxMessagesA( HXCEEDZIP hZip, xcdSfxMessages xIndex, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxMessagesW( HXCEEDZIP hZip, xcdSfxMessages xIndex, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxMessagesA( HXCEEDZIP hZip, xcdSfxMessages xIndex, const char* pszValue );

// SfxProgramGroup property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxProgramGroupW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxProgramGroupA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxProgramGroupW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxProgramGroupA( HXCEEDZIP hZip, const char* pszValue );

// SfxProgramGroupItems property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxProgramGroupItemsW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxProgramGroupItemsA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxProgramGroupItemsW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxProgramGroupItemsA( HXCEEDZIP hZip, const char* pszValue );

// SfxReadmeFile propertyf
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxReadmeFileW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxReadmeFileA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxReadmeFileW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxReadmeFileA( HXCEEDZIP hZip, const char* pszValue );

// SfxStrings property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxStringsW( HXCEEDZIP hZip, xcdSfxStrings xIndex, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxStringsA( HXCEEDZIP hZip, xcdSfxStrings xIndex, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxStringsW( HXCEEDZIP hZip, xcdSfxStrings xIndex, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxStringsA( HXCEEDZIP hZip, xcdSfxStrings xIndex, const char* pszValue );

// SkipIfExisting property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetSkipIfExisting( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSkipIfExisting( HXCEEDZIP hZip, BOOL bValue );

// SkipIfNotExisting property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetSkipIfNotExisting( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSkipIfNotExisting( HXCEEDZIP hZip, BOOL bValue );

// SkipIfOlderDate property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetSkipIfOlderDate( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSkipIfOlderDate( HXCEEDZIP hZip, BOOL bValue );

// SkipIfOlderVersion property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetSkipIfOlderVersion( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSkipIfOlderVersion( HXCEEDZIP hZip, BOOL bValue );

// SpanMultipleDisks property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSpanMultipleDisks( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSpanMultipleDisks( HXCEEDZIP hZip, UINT uValue );

// SplitSize property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetSplitSize( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetSplitSize( HXCEEDZIP hZip, DWORD dwValue );

// TempFolder property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetTempFolderW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetTempFolderA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetTempFolderW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetTempFolderA( HXCEEDZIP hZip, const char* pszValue );

// UnzipToFolder property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetUnzipToFolderW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetUnzipToFolderA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetUnzipToFolderW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetUnzipToFolderA( HXCEEDZIP hZip, const char* pszValue );

// UseTempFile property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetUseTempFile( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetUseTempFile( HXCEEDZIP hZip, BOOL bValue );

// ZipFilename property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetZipFilenameW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetZipFilenameA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetZipFilenameW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetZipFilenameA( HXCEEDZIP hZip, const char* pszValue );

// ZipOpenedFiles property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetZipOpenedFiles( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetZipOpenedFiles( HXCEEDZIP hZip, BOOL bValue );

// SfxFilesToCopy property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxFilesToCopyW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxFilesToCopyA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxFilesToCopyW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxFilesToCopyA( HXCEEDZIP hZip, const char* pszValue );

// SfxFilesToRegister property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxFilesToRegisterW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxFilesToRegisterA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxFilesToRegisterW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxFilesToRegisterA( HXCEEDZIP hZip, const char* pszValue );

// SfxRegistryKeys property
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxRegistryKeysW( HXCEEDZIP hZip, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetSfxRegistryKeysA( HXCEEDZIP hZip, char* pszBuffer,   UINT uMaxLength );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxRegistryKeysW( HXCEEDZIP hZip, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSetSfxRegistryKeysA( HXCEEDZIP hZip, const char* pszValue );

// DeleteZippedFiles property
XCD_IMPORT  BOOL  XCD_WINAPI  XzGetDeleteZippedFiles( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetDeleteZippedFiles( HXCEEDZIP hZip, BOOL bValue );

// FirstDiskFreeSpace property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetFirstDiskFreeSpace( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetFirstDiskFreeSpace( HXCEEDZIP hZip, DWORD dwValue );

// MinDiskFreeSpace property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetMinDiskFreeSpace( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetMinDiskFreeSpace( HXCEEDZIP hZip, DWORD dwValue );

// EventsToTrigger property
XCD_IMPORT  DWORD XCD_WINAPI  XzGetEventsToTrigger( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSetEventsToTrigger( HXCEEDZIP hZip, DWORD dwValue );


//
// XceedCompression property handling functions
//

XCD_IMPORT  UINT  XCD_WINAPI  XcGetEncryptionPasswordW( HXCEEDCMP hComp, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XcGetEncryptionPasswordA( HXCEEDCMP hComp, char*  pszEncryptionPassword, UINT uMaxLength );

XCD_IMPORT  void  XCD_WINAPI  XcSetEncryptionPasswordW( HXCEEDCMP hComp, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XcSetEncryptionPasswordA( HXCEEDCMP hComp, const char*  pszValue );

XCD_IMPORT  int   XCD_WINAPI  XcGetCompressionLevel( HXCEEDCMP hComp );
XCD_IMPORT  void  XCD_WINAPI  XcSetCompressionLevel( HXCEEDCMP hComp, int nValue );


//
// Exported API for XceedZip methods
//

// AddFilesToProcess method
XCD_IMPORT  void  XCD_WINAPI  XzAddFilesToProcessW( HXCEEDZIP hZip, const WCHAR* pwszFileMask );
XCD_IMPORT  void  XCD_WINAPI  XzAddFilesToProcessA( HXCEEDZIP hZip, const char* pszFileMask );

// AddFilesToExclude method
XCD_IMPORT  void  XCD_WINAPI  XzAddFilesToExcludeW( HXCEEDZIP hZip, const WCHAR* pwszFileMask );
XCD_IMPORT  void  XCD_WINAPI  XzAddFilesToExcludeA( HXCEEDZIP hZip, const char* pszFileMask );

// Convert method
XCD_IMPORT  int   XCD_WINAPI  XzConvertW( HXCEEDZIP hZip, const WCHAR* pwszDestFilename );
XCD_IMPORT  int   XCD_WINAPI  XzConvertA( HXCEEDZIP hZip, const char* pszDestFilename );

// XGetErrorDescription method
XCD_IMPORT  UINT  XCD_WINAPI  XzGetErrorDescriptionW( HXCEEDZIP hZip, xcdValueType xType, int nCode, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XzGetErrorDescriptionA( HXCEEDZIP hZip, xcdValueType xType, int nCode, char* pszBuffer,   UINT uMaxLength );

// XGetZipFileInformation method
XCD_IMPORT  int   XCD_WINAPI  XzGetZipFileInformation( HXCEEDZIP hZip, LONG* plNBFiles, LONG* plCompressedSize, LONG* plUncompressedSize, SHORT* pnCompressionRatio, BOOL* pbSpanned );

// ListZipConetnts method
XCD_IMPORT  int   XCD_WINAPI  XzListZipContents( HXCEEDZIP hZip );

// PreviewFiles method
XCD_IMPORT  int   XCD_WINAPI  XzPreviewFiles( HXCEEDZIP hZip, BOOL bCalcCompSize );

// RemoveFiles method
XCD_IMPORT  int   XCD_WINAPI  XzRemoveFiles( HXCEEDZIP hZip );

// SfxAddExtensionToAssociate method
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddExtensionToAssociateW( HXCEEDZIP hZip, const WCHAR* pwszDescription, const WCHAR* pwszExtension, const WCHAR* pwszApplication );
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddExtensionToAssociateA( HXCEEDZIP hZip, const char* pszDescription,   const char* pszExtension,   const char* pszApplication );

// SfxAddProgramGroupItem method
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddProgramGroupItemW( HXCEEDZIP hZip, const WCHAR* pwszApplication, const WCHAR* pwszDescription );
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddProgramGroupItemA( HXCEEDZIP hZip, const char* pszApplication,   const char* pszDescription );

// SfxClearButtons, SfxClearMessages and SfxClearStrings methods
XCD_IMPORT  void  XCD_WINAPI  XzSfxClearButtons( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSfxClearMessages( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSfxClearStrings( HXCEEDZIP hZip );

// SfxLoadConfig method
XCD_IMPORT  BOOL  XCD_WINAPI  XzSfxLoadConfigW( HXCEEDZIP hZip, const WCHAR* pwszConfigFilename );
XCD_IMPORT  BOOL  XCD_WINAPI  XzSfxLoadConfigA( HXCEEDZIP hZip, const char* pszConfigFilename );

// SfxSaveConfig method
XCD_IMPORT  BOOL  XCD_WINAPI  XzSfxSaveConfigW( HXCEEDZIP hZip, const WCHAR* pwszConfigFilename );
XCD_IMPORT  BOOL  XCD_WINAPI  XzSfxSaveConfigA( HXCEEDZIP hZip, const char* pszConfigFilename );

// SfxResetButtons, SfxResetMessages and SfxResetStrings methods
XCD_IMPORT  void  XCD_WINAPI  XzSfxResetButtons( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSfxResetMessages( HXCEEDZIP hZip );
XCD_IMPORT  void  XCD_WINAPI  XzSfxResetStrings( HXCEEDZIP hZip );

// TestZipFile method
XCD_IMPORT  int   XCD_WINAPI  XzTestZipFile( HXCEEDZIP hZip, BOOL bCheckCompressedData );

// Unzip method
XCD_IMPORT  int   XCD_WINAPI  XzUnzip( HXCEEDZIP hZip );

// Zip method
XCD_IMPORT  int   XCD_WINAPI  XzZip( HXCEEDZIP hZip );

// SfxAddExecuteAfter method
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddExecuteAfterW( HXCEEDZIP hZip, const WCHAR* pwszApplication, const WCHAR* pwszParameters );
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddExecuteAfterA( HXCEEDZIP hZip, const char* pszApplication,   const char* pszParameters );

// SfxAddFileToCopy method
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddFileToCopyW( HXCEEDZIP hZip, const WCHAR* pwszSource, const WCHAR* pwszDestination );
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddFileToCopyA( HXCEEDZIP hZip, const char* pszSource,   const char* pszDestination );

// SfxAddFileToRegister method
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddFileToRegisterW( HXCEEDZIP hZip, const WCHAR* pwszFilename );
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddFileToRegisterA( HXCEEDZIP hZip, const char* pszFilename );

// SfxAddRegistryKey method
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddRegistryKeyW( HXCEEDZIP hZip, const WCHAR* pwszKey, const WCHAR* pwszValueName, const WCHAR* pwszValue );
XCD_IMPORT  void  XCD_WINAPI  XzSfxAddRegistryKeyA( HXCEEDZIP hZip, const char* pszKey,   const char* pszValueName,   const char* pszValue );

// GetZipContents method
XCD_IMPORT  int   XCD_WINAPI  XzGetZipContents( HXCEEDZIP hZip, HXCEEDZIPITEMS* phItems );


//
// Exported API for XceedCompression methods
//

// GetErrorDescription method
XCD_IMPORT  UINT  XCD_WINAPI  XcGetErrorDescriptionW( HXCEEDCMP hComp, int nCode, WCHAR* pwszBuffer, UINT uMaxLength );
XCD_IMPORT  UINT  XCD_WINAPI  XcGetErrorDescriptionA( HXCEEDCMP hComp, int nCode, char* pszBuffer,   UINT uMaxLength );

// Compress method
XCD_IMPORT  int   XCD_WINAPI  XcCompress( HXCEEDCMP hComp, const BYTE* pcSource, DWORD dwSourceSize, BYTE** ppcCompressed, DWORD* pdwCompressedSize, BOOL bEndOfData );

// Uncompress method
XCD_IMPORT  int   XCD_WINAPI  XcUncompress( HXCEEDCMP hComp, const BYTE* pcSource, DWORD dwSourceSize, BYTE** ppcUncompressed, DWORD* pdwUncompressedSize, BOOL bEndOfData );

// CalculateCrc method
XCD_IMPORT  long  XCD_WINAPI  XcCalculateCrc( HXCEEDCMP hComp, const BYTE* pcData, DWORD dwDataSize, long lPreviousCrc );


//
// Exported API for XceedZipItems collection
//

XCD_IMPORT  BOOL  XCD_WINAPI  XziGetFirstItemW( HXCEEDZIPITEMS hItems, xcdListingFileParamsW* pxItem );
XCD_IMPORT  BOOL  XCD_WINAPI  XziGetFirstItemA( HXCEEDZIPITEMS hItems, xcdListingFileParamsA* pxItem );

XCD_IMPORT  BOOL  XCD_WINAPI  XziGetNextItemW( HXCEEDZIPITEMS hItems, xcdListingFileParamsW* pxItem );
XCD_IMPORT  BOOL  XCD_WINAPI  XziGetNextItemA( HXCEEDZIPITEMS hItems, xcdListingFileParamsA* pxItem );

#endif // _MSC_VER


////////////////////////////////////////////////////////////////////////////////
// Pointer to functions structure, for easy calls without using XCEEDZIP.LIB
//

typedef struct _XceedZipFunctions
{
  LPFNXCEEDZIPINITDLL                 lpfnXceedZipInitDLL;
  LPFNXCEEDZIPSHUTDOWNDLL             lpfnXceedZipShutdownDLL;
  LPFNXZCREATEXCEEDZIPW               lpfnXzCreateXceedZipW;
  LPFNXZCREATEXCEEDZIPA               lpfnXzCreateXceedZipA;
  LPFNXZDESTROYXCEEDZIP               lpfnXzDestroyXceedZip;
  LPFNXCCREATEXCEEDCOMPRESSIONW       lpfnXcCreateXceedCompressionW;
  LPFNXCCREATEXCEEDCOMPRESSIONA       lpfnXcCreateXceedCompressionA;
  LPFNXCDESTROYXCEEDCOMPRESSION       lpfnXcDestroyXceedCompression;
  LPFNXZSETXCEEDZIPCALLBACK           lpfnXzSetXceedZipCallback;
  LPFNXZSETXCEEDZIPWINDOW             lpfnXzSetXceedZipWindow;
  LPFNXZALLOC                         lpfnXzAlloc;
  LPFNXZFREE                          lpfnXzFree;
  LPFNXZGETABORT                      lpfnXzGetAbort;
  LPFNXZSETABORT                      lpfnXzSetAbort;
  LPFNXZGETBACKGROUNDPROCESSING       lpfnXzGetBackgroundProcessing;
  LPFNXZSETBACKGROUNDPROCESSING       lpfnXzSetBackgroundProcessing;
  LPFNXZGETBASEPATHW                  lpfnXzGetBasePathW;
  LPFNXZGETBASEPATHA                  lpfnXzGetBasePathA;
  LPFNXZSETBASEPATHW                  lpfnXzSetBasePathW;
  LPFNXZSETBASEPATHA                  lpfnXzSetBasePathA;
  LPFNXZGETCOMPRESSIONLEVEL           lpfnXzGetCompressionLevel;
  LPFNXZSETCOMPRESSIONLEVEL           lpfnXzSetCompressionLevel;
  LPFNXZGETCURRENTOPERATION           lpfnXzGetCurrentOperation;
  LPFNXZGETENCRYPTIONPASSWORDW        lpfnXzGetEncryptionPasswordW;
  LPFNXZGETENCRYPTIONPASSWORDA        lpfnXzGetEncryptionPasswordA;
  LPFNXZSETENCRYPTIONPASSWORDW        lpfnXzSetEncryptionPasswordW;
  LPFNXZSETENCRYPTIONPASSWORDA        lpfnXzSetEncryptionPasswordA;
  LPFNXZGETEXCLUDEDFILEATTRIBUTES     lpfnXzGetExcludedFileAttributes;
  LPFNXZSETEXCLUDEDFILEATTRIBUTES     lpfnXzSetExcludedFileAttributes;
  LPFNXZGETEXTRAHEADERS               lpfnXzGetExtraHeaders;
  LPFNXZSETEXTRAHEADERS               lpfnXzSetExtraHeaders;
  LPFNXZGETFILESTOEXCLUDEW            lpfnXzGetFilesToExcludeW;
  LPFNXZGETFILESTOEXCLUDEA            lpfnXzGetFilesToExcludeA;
  LPFNXZSETFILESTOEXCLUDEW            lpfnXzSetFilesToExcludeW;
  LPFNXZSETFILESTOEXCLUDEA            lpfnXzSetFilesToExcludeA;
  LPFNXZGETFILESTOPROCESSW            lpfnXzGetFilesToProcessW;
  LPFNXZGETFILESTOPROCESSA            lpfnXzGetFilesToProcessA;
  LPFNXZSETFILESTOPROCESSW            lpfnXzSetFilesToProcessW;
  LPFNXZSETFILESTOPROCESSA            lpfnXzSetFilesToProcessA;
  LPFNXZGETMAXDATETOPROCESS           lpfnXzGetMaxDateToProcess;
  LPFNXZSETMAXDATETOPROCESS           lpfnXzSetMaxDateToProcess;
  LPFNXZGETMAXSIZETOPROCESS           lpfnXzGetMaxSizeToProcess;
  LPFNXZSETMAXSIZETOPROCESS           lpfnXzSetMaxSizeToProcess;
  LPFNXZGETMINDATETOPROCESS           lpfnXzGetMinDateToProcess;
  LPFNXZSETMINDATETOPROCESS           lpfnXzSetMinDateToProcess;
  LPFNXZGETMINSIZETOPROCESS           lpfnXzGetMinSizeToProcess;
  LPFNXZSETMINSIZETOPROCESS           lpfnXzSetMinSizeToProcess;
  LPFNXZGETPRESERVEPATHS              lpfnXzGetPreservePaths;
  LPFNXZSETPRESERVEPATHS              lpfnXzSetPreservePaths;
  LPFNXZGETPROCESSSUBFOLDERS          lpfnXzGetProcessSubfolders;
  LPFNXZSETPROCESSSUBFOLDERS          lpfnXzSetProcessSubfolders;
  LPFNXZGETREQUIREDFILEATTRIBUTES     lpfnXzGetRequiredFileAttributes;
  LPFNXZSETREQUIREDFILEATTRIBUTES     lpfnXzSetRequiredFileAttributes;
  LPFNXZGETSFXBINARYMODULEW           lpfnXzGetSfxBinaryModuleW;
  LPFNXZGETSFXBINARYMODULEA           lpfnXzGetSfxBinaryModuleA;
  LPFNXZSETSFXBINARYMODULEW           lpfnXzSetSfxBinaryModuleW;
  LPFNXZSETSFXBINARYMODULEA           lpfnXzSetSfxBinaryModuleA;
  LPFNXZGETSFXBUTTONSW                lpfnXzGetSfxButtonsW;
  LPFNXZGETSFXBUTTONSA                lpfnXzGetSfxButtonsA;
  LPFNXZSETSFXBUTTONSW                lpfnXzSetSfxButtonsW;
  LPFNXZSETSFXBUTTONSA                lpfnXzSetSfxButtonsA;
  LPFNXZGETSFXDEFAULTPASSWORDW        lpfnXzGetSfxDefaultPasswordW;
  LPFNXZGETSFXDEFAULTPASSWORDA        lpfnXzGetSfxDefaultPasswordA;
  LPFNXZSETSFXDEFAULTPASSWORDW        lpfnXzSetSfxDefaultPasswordW;
  LPFNXZSETSFXDEFAULTPASSWORDA        lpfnXzSetSfxDefaultPasswordA;
  LPFNXZGETSFXDEFAULTUNZIPTOFOLDERW   lpfnXzGetSfxDefaultUnzipToFolderW;
  LPFNXZGETSFXDEFAULTUNZIPTOFOLDERA   lpfnXzGetSfxDefaultUnzipToFolderA;
  LPFNXZSETSFXDEFAULTUNZIPTOFOLDERW   lpfnXzSetSfxDefaultUnzipToFolderW;
  LPFNXZSETSFXDEFAULTUNZIPTOFOLDERA   lpfnXzSetSfxDefaultUnzipToFolderA;
  LPFNXZGETSFXEXISTINGFILEBEHAVIOR    lpfnXzGetSfxExistingFileBehavior;
  LPFNXZSETSFXEXISTINGFILEBEHAVIOR    lpfnXzSetSfxExistingFileBehavior;
  LPFNXZGETSFXEXTENSIONSTOASSOCIATEW  lpfnXzGetSfxExtensionsToAssociateW;
  LPFNXZGETSFXEXTENSIONSTOASSOCIATEA  lpfnXzGetSfxExtensionsToAssociateA;
  LPFNXZSETSFXEXTENSIONSTOASSOCIATEW  lpfnXzSetSfxExtensionsToAssociateW;
  LPFNXZSETSFXEXTENSIONSTOASSOCIATEA  lpfnXzSetSfxExtensionsToAssociateA;
  LPFNXZGETSFXEXECUTEAFTERW           lpfnXzGetSfxExecuteAfterW;
  LPFNXZGETSFXEXECUTEAFTERA           lpfnXzGetSfxExecuteAfterA;
  LPFNXZSETSFXEXECUTEAFTERW           lpfnXzSetSfxExecuteAfterW;
  LPFNXZSETSFXEXECUTEAFTERA           lpfnXzSetSfxExecuteAfterA;
  LPFNXZGETSFXICONFILENAMEW           lpfnXzGetSfxIconFilenameW;
  LPFNXZGETSFXICONFILENAMEA           lpfnXzGetSfxIconFilenameA;
  LPFNXZSETSFXICONFILENAMEW           lpfnXzSetSfxIconFilenameW;
  LPFNXZSETSFXICONFILENAMEA           lpfnXzSetSfxIconFilenameA;
  LPFNXZGETSFXINSTALLMODE             lpfnXzGetSfxInstallMode;
  LPFNXZSETSFXINSTALLMODE             lpfnXzSetSfxInstallMode;
  LPFNXZGETSFXMESSAGESW               lpfnXzGetSfxMessagesW;
  LPFNXZGETSFXMESSAGESA               lpfnXzGetSfxMessagesA;
  LPFNXZSETSFXMESSAGESW               lpfnXzSetSfxMessagesW;
  LPFNXZSETSFXMESSAGESA               lpfnXzSetSfxMessagesA;
  LPFNXZGETSFXPROGRAMGROUPW           lpfnXzGetSfxProgramGroupW;
  LPFNXZGETSFXPROGRAMGROUPA           lpfnXzGetSfxProgramGroupA;
  LPFNXZSETSFXPROGRAMGROUPW           lpfnXzSetSfxProgramGroupW;
  LPFNXZSETSFXPROGRAMGROUPA           lpfnXzSetSfxProgramGroupA;
  LPFNXZGETSFXPROGRAMGROUPITEMSW      lpfnXzGetSfxProgramGroupItemsW;
  LPFNXZGETSFXPROGRAMGROUPITEMSA      lpfnXzGetSfxProgramGroupItemsA;
  LPFNXZSETSFXPROGRAMGROUPITEMSW      lpfnXzSetSfxProgramGroupItemsW;
  LPFNXZSETSFXPROGRAMGROUPITEMSA      lpfnXzSetSfxProgramGroupItemsA;
  LPFNXZGETSFXREADMEFILEW             lpfnXzGetSfxReadmeFileW;
  LPFNXZGETSFXREADMEFILEA             lpfnXzGetSfxReadmeFileA;
  LPFNXZSETSFXREADMEFILEW             lpfnXzSetSfxReadmeFileW;
  LPFNXZSETSFXREADMEFILEA             lpfnXzSetSfxReadmeFileA;
  LPFNXZGETSFXSTRINGSW                lpfnXzGetSfxStringsW;
  LPFNXZGETSFXSTRINGSA                lpfnXzGetSfxStringsA;
  LPFNXZSETSFXSTRINGSW                lpfnXzSetSfxStringsW;
  LPFNXZSETSFXSTRINGSA                lpfnXzSetSfxStringsA;
  LPFNXZGETSKIPIFEXISTING             lpfnXzGetSkipIfExisting;
  LPFNXZSETSKIPIFEXISTING             lpfnXzSetSkipIfExisting;
  LPFNXZGETSKIPIFNOTEXISTING          lpfnXzGetSkipIfNotExisting;
  LPFNXZSETSKIPIFNOTEXISTING          lpfnXzSetSkipIfNotExisting;
  LPFNXZGETSKIPIFOLDERDATE            lpfnXzGetSkipIfOlderDate;
  LPFNXZSETSKIPIFOLDERDATE            lpfnXzSetSkipIfOlderDate;
  LPFNXZGETSKIPIFOLDERVERSION         lpfnXzGetSkipIfOlderVersion;
  LPFNXZSETSKIPIFOLDERVERSION         lpfnXzSetSkipIfOlderVersion;
  LPFNXZGETSPANMULTIPLEDISKS          lpfnXzGetSpanMultipleDisks;
  LPFNXZSETSPANMULTIPLEDISKS          lpfnXzSetSpanMultipleDisks;
  LPFNXZGETSPLITSIZE                  lpfnXzGetSplitSize;
  LPFNXZSETSPLITSIZE                  lpfnXzSetSplitSize;
  LPFNXZGETTEMPFOLDERW                lpfnXzGetTempFolderW;
  LPFNXZGETTEMPFOLDERA                lpfnXzGetTempFolderA;
  LPFNXZSETTEMPFOLDERW                lpfnXzSetTempFolderW;
  LPFNXZSETTEMPFOLDERA                lpfnXzSetTempFolderA;
  LPFNXZGETUNZIPTOFOLDERW             lpfnXzGetUnzipToFolderW;
  LPFNXZGETUNZIPTOFOLDERA             lpfnXzGetUnzipToFolderA;
  LPFNXZSETUNZIPTOFOLDERW             lpfnXzSetUnzipToFolderW;
  LPFNXZSETUNZIPTOFOLDERA             lpfnXzSetUnzipToFolderA;
  LPFNXZGETUSETEMPFILE                lpfnXzGetUseTempFile;
  LPFNXZSETUSETEMPFILE                lpfnXzSetUseTempFile;
  LPFNXZGETZIPFILENAMEW               lpfnXzGetZipFilenameW;
  LPFNXZGETZIPFILENAMEA               lpfnXzGetZipFilenameA;
  LPFNXZSETZIPFILENAMEW               lpfnXzSetZipFilenameW;
  LPFNXZSETZIPFILENAMEA               lpfnXzSetZipFilenameA;
  LPFNXZGETZIPOPENEDFILES             lpfnXzGetZipOpenedFiles;
  LPFNXZSETZIPOPENEDFILES             lpfnXzSetZipOpenedFiles;
  LPFNXCGETENCRYPTIONPASSWORDW        lpfnXcGetEncryptionPasswordW;
  LPFNXCGETENCRYPTIONPASSWORDA        lpfnXcGetEncryptionPasswordA;
  LPFNXCSETENCRYPTIONPASSWORDW        lpfnXcSetEncryptionPasswordW;
  LPFNXCSETENCRYPTIONPASSWORDA        lpfnXcSetEncryptionPasswordA;
  LPFNXCGETCOMPRESSIONLEVEL           lpfnXcGetCompressionLevel;
  LPFNXCSETCOMPRESSIONLEVEL           lpfnXcSetCompressionLevel;
  LPFNXZADDFILESTOPROCESSW            lpfnXzAddFilesToProcessW;
  LPFNXZADDFILESTOPROCESSA            lpfnXzAddFilesToProcessA;
  LPFNXZADDFILESTOEXCLUDEW            lpfnXzAddFilesToExcludeW;
  LPFNXZADDFILESTOEXCLUDEA            lpfnXzAddFilesToExcludeA;
  LPFNXZCONVERTW                      lpfnXzConvertW;
  LPFNXZCONVERTA                      lpfnXzConvertA;
  LPFNXZGETERRORDESCRIPTIONW          lpfnXzGetErrorDescriptionW;
  LPFNXZGETERRORDESCRIPTIONA          lpfnXzGetErrorDescriptionA;
  LPFNXZGETZIPFILEINFORMATION         lpfnXzGetZipFileInformation;
  LPFNXZLISTZIPCONTENTS               lpfnXzListZipContents;
  LPFNXZPREVIEWFILES                  lpfnXzPreviewFiles;
  LPFNXZREMOVEFILES                   lpfnXzRemoveFiles;
  LPFNXZSFXADDEXTENSIONTOASSOCIATEW   lpfnXzSfxAddExtensionToAssociateW;
  LPFNXZSFXADDEXTENSIONTOASSOCIATEA   lpfnXzSfxAddExtensionToAssociateA;
  LPFNXZSFXADDPROGRAMGROUPITEMW       lpfnXzSfxAddProgramGroupItemW;
  LPFNXZSFXADDPROGRAMGROUPITEMA       lpfnXzSfxAddProgramGroupItemA;
  LPFNXZSFXCLEARBUTTONS               lpfnXzSfxClearButtons;
  LPFNXZSFXCLEARMESSAGES              lpfnXzSfxClearMessages;
  LPFNXZSFXCLEARSTRINGS               lpfnXzSfxClearStrings;
  LPFNXZSFXLOADCONFIGW                lpfnXzSfxLoadConfigW;
  LPFNXZSFXLOADCONFIGA                lpfnXzSfxLoadConfigA;
  LPFNXZSFXSAVECONFIGW                lpfnXzSfxSaveConfigW;
  LPFNXZSFXSAVECONFIGA                lpfnXzSfxSaveConfigA;
  LPFNXZSFXRESETBUTTONS               lpfnXzSfxResetButtons;
  LPFNXZSFXRESETMESSAGES              lpfnXzSfxResetMessages;
  LPFNXZSFXRESETSTRINGS               lpfnXzSfxResetStrings;
  LPFNXZTESTZIPFILE                   lpfnXzTestZipFile;
  LPFNXZUNZIP                         lpfnXzUnzip;
  LPFNXZZIP                           lpfnXzZip;
  LPFNXCGETERRORDESCRIPTIONW          lpfnXcGetErrorDescriptionW;
  LPFNXCGETERRORDESCRIPTIONA          lpfnXcGetErrorDescriptionA;
  LPFNXCCOMPRESS                      lpfnXcCompress;
  LPFNXCUNCOMPRESS                    lpfnXcUncompress;
  LPFNXCCALCULATECRC                  lpfnXcCalculateCrc;
  LPFNXZGETSFXFILESTOCOPYW            lpfnXzGetSfxFilesToCopyW;
  LPFNXZGETSFXFILESTOCOPYA            lpfnXzGetSfxFilesToCopyA;
  LPFNXZSETSFXFILESTOCOPYW            lpfnXzSetSfxFilesToCopyW;
  LPFNXZSETSFXFILESTOCOPYA            lpfnXzSetSfxFilesToCopyA;
  LPFNXZGETSFXFILESTOREGISTERW        lpfnXzGetSfxFilesToRegisterW;
  LPFNXZGETSFXFILESTOREGISTERA        lpfnXzGetSfxFilesToRegisterA;
  LPFNXZSETSFXFILESTOREGISTERW        lpfnXzSetSfxFilesToRegisterW;
  LPFNXZSETSFXFILESTOREGISTERA        lpfnXzSetSfxFilesToRegisterA;
  LPFNXZGETSFXREGISTRYKEYSW           lpfnXzGetSfxRegistryKeysW;
  LPFNXZGETSFXREGISTRYKEYSA           lpfnXzGetSfxRegistryKeysA;
  LPFNXZSETSFXREGISTRYKEYSW           lpfnXzSetSfxRegistryKeysW;
  LPFNXZSETSFXREGISTRYKEYSA           lpfnXzSetSfxRegistryKeysA;
  LPFNXZGETDELETEZIPPEDFILES          lpfnXzGetDeleteZippedFiles;
  LPFNXZSETDELETEZIPPEDFILES          lpfnXzSetDeleteZippedFiles;
  LPFNXZSFXADDEXECUTEAFTERW           lpfnXzSfxAddExecuteAfterW;
  LPFNXZSFXADDEXECUTEAFTERA           lpfnXzSfxAddExecuteAfterA;
  LPFNXZSFXADDFILETOCOPYW             lpfnXzSfxAddFileToCopyW;
  LPFNXZSFXADDFILETOCOPYA             lpfnXzSfxAddFileToCopyA;
  LPFNXZSFXADDFILETOREGISTERW         lpfnXzSfxAddFileToRegisterW;
  LPFNXZSFXADDFILETOREGISTERA         lpfnXzSfxAddFileToRegisterA;
  LPFNXZSFXADDREGISTRYKEYW            lpfnXzSfxAddRegistryKeyW;
  LPFNXZSFXADDREGISTRYKEYA            lpfnXzSfxAddRegistryKeyA;
  LPFNXZGETFIRSTDISKFREESPACE         lpfnXzGetFirstDiskFreeSpace;
  LPFNXZSETFIRSTDISKFREESPACE         lpfnXzSetFirstDiskFreeSpace;
  LPFNXZGETMINDISKFREESPACE           lpfnXzGetMinDiskFreeSpace;
  LPFNXZSETMINDISKFREESPACE           lpfnXzSetMinDiskFreeSpace;
  LPFNXZGETEVENTSTOTRIGGER            lpfnXzGetEventsToTrigger;
  LPFNXZSETEVENTSTOTRIGGER            lpfnXzSetEventsToTrigger;
  LPFNXZIDESTROYXCEEDZIPITEMS         lpfnXziDestroyXceedZipItems;
  LPFNXZGETZIPCONTENTS                lpfnXzGetZipContents;
  LPFNXZIGETFIRSTITEMW                lpfnXziGetFirstItemW;
  LPFNXZIGETFIRSTITEMA                lpfnXziGetFirstItemA;
  LPFNXZIGETNEXTITEMW                 lpfnXziGetNextItemW;
  LPFNXZIGETNEXTITEMA                 lpfnXziGetNextItemA;
} XceedZipFunctions;


////////////////////////////////////////////////////////////////////////////////
// Unicode / Ansi defines
//

// Event parameters' structures
#ifdef UNICODE
typedef xcdListingFileParamsW               xcdListingFileParams;
typedef xcdPreviewingFileParamsW            xcdPreviewingFileParams;
typedef xcdZipPreprocessingFileParamsW      xcdZipPreprocessingFileParams;
typedef xcdUnzipPreprocessingFileParamsW    xcdUnzipPreprocessingFileParams;
typedef xcdSkippingFileParamsW              xcdSkippingFileParams;
typedef xcdRemovingFileParamsW              xcdRemovingFileParams;
typedef xcdTestingFileParamsW               xcdTestingFileParams;
typedef xcdFileStatusParamsW                xcdFileStatusParams;
typedef xcdZipCommentParamsW                xcdZipCommentParams;
typedef xcdQueryMemoryFileParamsW           xcdQueryMemoryFileParams;
typedef xcdZippingMemoryFileParamsW         xcdZippingMemoryFileParams;
typedef xcdUnzippingMemoryFileParamsW       xcdUnzippingMemoryFileParams;
typedef xcdWarningParamsW                   xcdWarningParams;
typedef xcdInvalidPasswordParamsW           xcdInvalidPasswordParams;
typedef xcdReplacingFileParamsW             xcdReplacingFileParams;
typedef xcdDeletingFileParamsW              xcdDeletingFileParams;
typedef xcdConvertPreprocessingFileParamsW  xcdxcdConvertPreprocessingFileParams;
#else
typedef xcdListingFileParamsA               xcdListingFileParams;
typedef xcdPreviewingFileParamsA            xcdPreviewingFileParams;
typedef xcdZipPreprocessingFileParamsA      xcdZipPreprocessingFileParams;
typedef xcdUnzipPreprocessingFileParamsA    xcdUnzipPreprocessingFileParams;
typedef xcdSkippingFileParamsA              xcdSkippingFileParams;
typedef xcdRemovingFileParamsA              xcdRemovingFileParams;
typedef xcdTestingFileParamsA               xcdTestingFileParams;
typedef xcdFileStatusParamsA                xcdFileStatusParams;
typedef xcdZipCommentParamsA                xcdZipCommentParams;
typedef xcdQueryMemoryFileParamsA           xcdQueryMemoryFileParams;
typedef xcdZippingMemoryFileParamsA         xcdZippingMemoryFileParams;
typedef xcdUnzippingMemoryFileParamsA       xcdUnzippingMemoryFileParams;
typedef xcdWarningParamsA                   xcdWarningParams;
typedef xcdInvalidPasswordParamsA           xcdInvalidPasswordParams;
typedef xcdReplacingFileParamsA             xcdReplacingFileParams;
typedef xcdDeletingFileParamsA              xcdDeletingFileParams;
typedef xcdConvertPreprocessingFileParamsA  xcdxcdConvertPreprocessingFileParams;
#endif

// XceedZip creation
#ifdef UNICODE
#define XzCreateXceedZip              XzCreateXceedZipW
#else
#define XzCreateXceedZip              XzCreateXceedZipA
#endif

// XceedZip properties
#ifdef UNICODE
#define XzGetBasePath                 XzGetBasePathW
#define XzSetBasePath                 XzSetBasePathW
#define XzGetEncryptionPassword       XzGetEncryptionPasswordW
#define XzSetEncryptionPassword       XzSetEncryptionPasswordW
#define XzGetFilesToExclude           XzGetFilesToExcludeW
#define XzSetFilesToExclude           XzSetFilesToExcludeW
#define XzGetFilesToProcess           XzGetFilesToProcessW
#define XzSetFilesToProcess           XzSetFilesToProcessW
#define XzGetSfxBinaryModule          XzGetSfxBinaryModuleW
#define XzSetSfxBinaryModule          XzSetSfxBinaryModuleW
#define XzGetSfxButtons               XzGetSfxButtonsW
#define XzSetSfxButtons               XzSetSfxButtonsW
#define XzGetSfxDefaultPassword       XzGetSfxDefaultPasswordW
#define XzSetSfxDefaultPassword       XzSetSfxDefaultPasswordW
#define XzGetSfxDefaultUnzipToFolder  XzGetSfxDefaultUnzipToFolderW
#define XzSetSfxDefaultUnzipToFolder  XzSetSfxDefaultUnzipToFolderW
#define XzGetSfxExtensionsToAssociate XzGetSfxExtensionsToAssociateW
#define XzSetSfxExtensionsToAssociate XzSetSfxExtensionsToAssociateW
#define XzGetSfxExecuteAfter          XzGetSfxExecuteAfterW
#define XzSetSfxExecuteAfter          XzSetSfxExecuteAfterW
#define XzGetSfxIconFilename          XzGetSfxIconFilenameW
#define XzSetSfxIconFilename          XzSetSfxIconFilenameW
#define XzGetSfxMessages              XzGetSfxMessagesW
#define XzSetSfxMessages              XzSetSfxMessagesW
#define XzGetSfxProgramGroup          XzGetSfxProgramGroupW
#define XzSetSfxProgramGroup          XzSetSfxProgramGroupW
#define XzGetSfxProgramGroupItems     XzGetSfxProgramGroupItemsW
#define XzSetSfxProgramGroupItems     XzSetSfxProgramGroupItemsW
#define XzGetSfxReadmeFile            XzGetSfxReadmeFileW
#define XzSetSfxReadmeFile            XzSetSfxReadmeFileW
#define XzGetSfxStrings               XzGetSfxStringsW
#define XzSetSfxStrings               XzSetSfxStringsW
#define XzGetTempFolder               XzGetTempFolderW
#define XzSetTempFolder               XzSetTempFolderW
#define XzGetUnzipToFolder            XzGetUnzipToFolderW
#define XzSetUnzipToFolder            XzSetUnzipToFolderW
#define XzGetZipFilename              XzGetZipFilenameW
#define XzSetZipFilename              XzSetZipFilenameW
#define XzGetSfxFilesToCopy           XzGetSfxFilesToCopyW
#define XzSetSfxFilesToCopy           XzSetSfxFilesToCopyW
#define XzGetSfxFilesToRegister       XzGetSfxFilesToRegisterW
#define XzSetSfxFilesToRegister       XzSetSfxFilesToRegisterW
#define XzGetSfxRegistryKeys          XzGetSfxRegistryKeysW
#define XzSetSfxRegistryKeys          XzSetSfxRegistryKeysW
#else
#define XzGetBasePath                 XzGetBasePathA
#define XzSetBasePath                 XzSetBasePathA
#define XzGetEncryptionPassword       XzGetEncryptionPasswordA
#define XzSetEncryptionPassword       XzSetEncryptionPasswordA
#define XzGetFilesToExclude           XzGetFilesToExcludeA
#define XzSetFilesToExclude           XzSetFilesToExcludeA
#define XzGetFilesToProcess           XzGetFilesToProcessA
#define XzSetFilesToProcess           XzSetFilesToProcessA
#define XzGetSfxBinaryModule          XzGetSfxBinaryModuleA
#define XzSetSfxBinaryModule          XzSetSfxBinaryModuleA
#define XzGetSfxButtons               XzGetSfxButtonsA
#define XzSetSfxButtons               XzSetSfxButtonsA
#define XzGetSfxDefaultPassword       XzGetSfxDefaultPasswordA
#define XzSetSfxDefaultPassword       XzSetSfxDefaultPasswordA
#define XzGetSfxDefaultUnzipToFolder  XzGetSfxDefaultUnzipToFolderA
#define XzSetSfxDefaultUnzipToFolder  XzSetSfxDefaultUnzipToFolderA
#define XzGetSfxExtensionsToAssociate XzGetSfxExtensionsToAssociateA
#define XzSetSfxExtensionsToAssociate XzSetSfxExtensionsToAssociateA
#define XzGetSfxExecuteAfter          XzGetSfxExecuteAfterA
#define XzSetSfxExecuteAfter          XzSetSfxExecuteAfterA
#define XzGetSfxIconFilename          XzGetSfxIconFilenameA
#define XzSetSfxIconFilename          XzSetSfxIconFilenameA
#define XzGetSfxMessages              XzGetSfxMessagesA
#define XzSetSfxMessages              XzSetSfxMessagesA
#define XzGetSfxProgramGroup          XzGetSfxProgramGroupA
#define XzSetSfxProgramGroup          XzSetSfxProgramGroupA
#define XzGetSfxProgramGroupItems     XzGetSfxProgramGroupItemsA
#define XzSetSfxProgramGroupItems     XzSetSfxProgramGroupItemsA
#define XzGetSfxReadmeFile            XzGetSfxReadmeFileA
#define XzSetSfxReadmeFile            XzSetSfxReadmeFileA
#define XzGetSfxStrings               XzGetSfxStringsA
#define XzSetSfxStrings               XzSetSfxStringsA
#define XzGetTempFolder               XzGetTempFolderA
#define XzSetTempFolder               XzSetTempFolderA
#define XzGetUnzipToFolder            XzGetUnzipToFolderA
#define XzSetUnzipToFolder            XzSetUnzipToFolderA
#define XzGetZipFilename              XzGetZipFilenameA
#define XzSetZipFilename              XzSetZipFilenameA
#define XzGetSfxFilesToCopy           XzGetSfxFilesToCopyA
#define XzSetSfxFilesToCopy           XzSetSfxFilesToCopyA
#define XzGetSfxFilesToRegister       XzGetSfxFilesToRegisterA
#define XzSetSfxFilesToRegister       XzSetSfxFilesToRegisterA
#define XzGetSfxRegistryKeys          XzGetSfxRegistryKeysA
#define XzSetSfxRegistryKeys          XzSetSfxRegistryKeysA
#endif

// XceedCompression creation
#ifdef UNICODE
#define XcCreateXceedCompression  XcCreateXceedCompressionW
#else
#define XcCreateXceedCompression  XcCreateXceedCompressionA
#endif

// XceedCompression properties
#ifdef UNICODE
#define XcGetEncryptionPassword XcGetEncryptionPasswordW
#define XcSetEncryptionPassword XcSetEncryptionPasswordW
#else
#define XcGetEncryptionPassword XcGetEncryptionPasswordA
#define XcSetEncryptionPassword XcSetEncryptionPasswordA
#endif

// XceedZip methods
#ifdef UNICODE
#define XzAddFilesToProcess           XzAddFilesToProcessW
#define XzAddFilesToExclude           XzAddFilesToExcludeW
#define XzConvert                     XzConvertW
#define XzGetErrorDescription         XzGetErrorDescriptionW
#define XzSfxAddExtensionToAssociate  XzSfxAddExtensionToAssociateW
#define XzSfxAddProgramGroupItem      XzSfxAddProgramGroupItemW
#define XzSfxLoadConfig               XzSfxLoadConfigW
#define XzSfxSaveConfig               XzSfxSaveConfigW
#define XzSfxAddExecuteAfter          XzSfxAddExecuteAfterW
#define XzSfxAddFileToCopy            XzSfxAddFileToCopyW
#define XzSfxAddFileToRegister        XzSfxAddFileToRegisterW
#define XzSfxAddRegistryKey           XzSfxAddRegistryKeyW
#else
#define XzAddFilesToProcess           XzAddFilesToProcessA
#define XzAddFilesToExclude           XzAddFilesToExcludeA
#define XzConvert                     XzConvertA
#define XzGetErrorDescription         XzGetErrorDescriptionA
#define XzSfxAddExtensionToAssociate  XzSfxAddExtensionToAssociateA
#define XzSfxAddProgramGroupItem      XzSfxAddProgramGroupItemA
#define XzSfxLoadConfig               XzSfxLoadConfigA
#define XzSfxSaveConfig               XzSfxSaveConfigA
#define XzSfxAddExecuteAfter          XzSfxAddExecuteAfterA
#define XzSfxAddFileToCopy            XzSfxAddFileToCopyA
#define XzSfxAddFileToRegister        XzSfxAddFileToRegisterA
#define XzSfxAddRegistryKey           XzSfxAddRegistryKeyA
#endif

// XceedCompression methods
#ifdef UNICODE
#define XcGetErrorDescription     XcGetErrorDescriptionW
#else
#define XcGetErrorDescription     XcGetErrorDescriptionA
#endif

// XceedZipItems methods
#ifdef UNICODE
#define XziGetFirstItem           XziGetFirstItemW
#define XziGetNextItem            XziGetNextItemW
#else
#define XziGetFirstItem           XziGetFirstItemW
#define XziGetNextItem            XziGetNextItemW
#endif

////////////////////////////////////////////////////////////////////////////////
// Global variables for use when linking with XCEEDZIP.LIB
// (Visual C++ only)
//

#ifdef _MSC_VER
extern XCD_IMPORT XceedZipFunctions  g_xzFunctions;
#endif


#ifdef __cplusplus
} // extern "C"
#endif

#endif // __XCEEDZIPAPI_H__

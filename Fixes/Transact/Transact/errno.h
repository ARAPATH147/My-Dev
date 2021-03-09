#if !_ERRNO_INCLUDED
/* Values_for_errno (see status.cf) */

enum Values_for_errno {
   No_error_occurred,
   Error_invalid_function,
   Error_file_not_found,
   Error_path_not_found,Error_too_many_open_files,Error_access_denied,
   Error_invalid_handle,Error_arena_trashed,Error_not_enough_memory,
   Error_invalid_block,Error_bad_environment,Error_bad_format,
   Error_invalid_access,Error_invalid_data,Error_reserved,
   Error_invalid_drive,Error_current_directory,Error_not_same_device,
   Error_no_more_files,Error_invalid_radix,Error_numeric_read_failed,
   Error_write_failed,Error_eof_encountered,Error_out_of_domain,
   Error_out_of_range};
#define EDOM Error_out_of_domain
#define ERANGE Error_out_of_range
extern int errno;
#define _ERRNO_INCLUDED 1
#endif

# Format specifications

Razor syntax is overkill in this use case, so let's try to just String.Format()

## ErrorView.html
- {0}: Page title
- {1}: Subheader e.g. "Error 404"
- {2}: Additional text e.g. "file /Example.txt was not found"

## FsView.html
- {0}: Page title
- {1}: Subheader 
- {2}: Navigation breadcrumbs
- {3}: Directories
- {4}: Files

## DirectoryWidget.html
- {0}: Browse link
- {1}: Name
- {2}: Last modified

## FileWidget.html
- {0}: Preview link
- {1}: Download link
- {2}: Name
- {3}: Size
- {4}: Last modified

## BreadcrumbWidget.html
- {0}: Link
- {1}: Name



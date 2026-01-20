## Running the Application

To run the application successfully, **both input files must be placed in the output directory**.

### Required Files
- **Cloud services database (CSV)**  
  Example: `ServiceDBv1.csv`
- **Firewall log file**  
  Example: `firewall.log`

### File Location
Both files **must be located in the application output folder**, for example: bin/Debug/net8.0/

## Project Overview

This project analyzes firewall logs to identify which internal IPs in an organization are using known cloud services.

### What It Does
- Parses large firewall log files (streamed in chunks)
- Correlates log records with a cloud services database (CSV)
- Resolves missing domains using reverse DNS (with caching)
- Applies configurable filters (IP include/exclude, user regex)
- Outputs **distinct internal IPs per cloud service**

### Design Highlights
- **Streaming & chunk-based processing** to handle large files efficiently
- **Parallel processing** for improved performance
- **Clear separation of concerns** (parsing, filtering, processing, aggregation)
- **Minimal memory usage** by aggregating results incrementally
- **Extensible filter architecture** for future rule types

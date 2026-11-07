#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

#include "base64.h"
#include "version.h"

const char* COMPILE_INFO = "Compiled on " __DATE__ " at " __TIME__;

int main(int argc, char *argv[]) {
	if (argc == 2) {
		if (strcmp(argv[1], "-h") == 0 || strcmp(argv[1], "--help") == 0) {
			printf(
				"Usage: b64 [options] [file1 file2 ...]\n"
				"\n"
				"Base64 encoder/decoder. Reads input files or stdin, writes to a single output or stdout.\n"
				"\n"
				"Options:\n"
				"  -o <file>                write output to <file> instead of stdout\n\n"

				"  -d    --decode           decode input instead of encoding\n"
				"  -i    --ignore-garbage   when decoding, ignore non-alphabet characters\n"
				"  -u    --url-safe         use Base64URL encoding\n"
				"  -w    --wrap             wrap encoded lines after 80 characters\n\n"

				"  -h    --help             show this help message and exit\n"
				"  -l    --list             list all available encodings\n"
				"  -v    --version          show version information and exit\n"
				"\n"
				"If no input files are given, input is read from stdin.\n"
				"\n"
				"Examples:\n"
				"  b64 input.txt > out.b64\n"
				"  b64 -d -o decoded.bin out.b64\n"
				"  cat data | b64 > encoded.txt\n"
				"\n"
			);

			return 0;
		} else if (strcmp(argv[1], "-l") == 0 || strcmp(argv[1], "--list") == 0) {
			printf("List of available encodings:\n");
			printf("  Default                         '%s'\n", b64_table_default);
			printf("  URL Safe    -u    --url-safe    '%s'\n", b64_table_url_safe);
			return 0;
		} else if (strcmp(argv[1], "-v") == 0 || strcmp(argv[1], "--version") == 0) {
			printf("%s - %s - %s\n", APP_NAME, VERSION_FULL_STR, COMPILE_INFO);
			return 0;
		}
	}

	int error = 0;
	bool decode = false;
	bool ignore_garbage = false; // not implemented yet, may not be needed
	int words_per_line = 0;
	const char *b64_table = b64_table_default;

	FILE *out = stdout;
	char **infiles = malloc(argc * sizeof(char*));
	int n_infiles = 0;

	// Parse args
	for (int i = 1; i < argc; i++) {
		if (strcmp(argv[i], "-d") == 0 || strcmp(argv[i], "--decode") == 0) {
			decode = true;
		} else if (strcmp(argv[i], "-i") == 0 || strcmp(argv[i], "--ignore-garbage") == 0) {
			ignore_garbage = true;
		} else if (strcmp(argv[i], "-u") == 0 || strcmp(argv[i], "--url-safe") == 0) {
			b64_table = b64_table_url_safe;
		} else if (strcmp(argv[i], "-w") == 0 || strcmp(argv[i], "--wrap") == 0) {
			words_per_line = 20;
		} else if (strcmp(argv[i], "-o") == 0 && i + 1 < argc) {
			out = fopen(argv[++i], decode ? "wb" : "w");
			if (!out) {
				perror("fopen output");
				error = 1;
				break;
			}
		} else if (argv[i][0] == '-') {
			fprintf(stderr, "Unknown option: '%s'\n", argv[i]);
			error = 1;
			break;
		} else {
			infiles[n_infiles++] = argv[i];
		}
	}

	if (error) {
		if (out != stdout) fclose(out);
		free(infiles);
		return error;
	}

	// Read from stdin if no input files given
	if (n_infiles == 0) {
		if (decode) base64_decode(stdin, out, b64_table);
		else base64_encode(stdin, out, b64_table, words_per_line);
	} else {
		for (int i = 0; i < n_infiles; ++i) {
			FILE *in = fopen(infiles[i], "rb");
			if (!in) {
				perror(infiles[i]);
				if (out != stdout) fclose(out);
				free(infiles);
				return 1;
			}

			if (decode) {
				error = base64_decode(in, out, b64_table);
			} else {
				base64_encode(in, out, b64_table, words_per_line);
			}

			fclose(in);
			if (error) break;
		}
	}

	if (out != stdout) fclose(out);
	free(infiles);
	return error;
}

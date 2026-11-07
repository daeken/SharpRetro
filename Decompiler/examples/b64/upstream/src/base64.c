#include "base64.h"



void base64_encode(FILE *in, FILE *out, const char *b64_table, int words_per_line) {
	unsigned char inbuf[3], outbuf[5];
	outbuf[4] = '\n'; // outbuf is "generally" only 4 bytes

	size_t bytes;
	int words_written = 0;

	while ((bytes = fread(inbuf, 1, 3, in)) > 0) {
		outbuf[0] = b64_table[inbuf[0] >> 2];
		outbuf[1] = b64_table[((inbuf[0] & 0x03) << 4) | (bytes > 1 ? inbuf[1] >> 4 : 0)];
		outbuf[2] = (bytes > 1) ? b64_table[((inbuf[1] & 0x0F) << 2) | (bytes > 2 ? inbuf[2] >> 6 : 0)] : '=';
		outbuf[3] = (bytes > 2) ? b64_table[inbuf[2] & 0x3F] : '=';

		int n = (words_per_line > 0 && (words_written+1) % words_per_line == 0) ? 5 : 4;
		fwrite(outbuf, 1, n, out);
		words_written++;
	}
}

int base64_index(char c, const char *b64_table) {
	if (c >= 'A' && c <= 'Z') return c - 'A';
	if (c >= 'a' && c <= 'z') return c - 'a' + 26;
	if (c >= '0' && c <= '9') return c - '0' + 52;
	if (c == b64_table[62]) return 62;
	if (c == b64_table[63]) return 63;
	return -1;
}

int base64_decode(FILE *in, FILE *out, const char *b64_table) {
	unsigned char inbuf[4], outbuf[3];
	int val[4];
	size_t i, bytes;
	int padding = 0;
	int ch;

	while (1) {
		i = 0;
		while (i < 4) {
			ch = fgetc(in);
			if (ch == EOF) {
				break;
			} else if (ch == '\n' || ch == '\r' || ch == ' ' || ch == '\t') {
				continue; // skip whitespace
			} else if (ch == '=') {
				padding++;
				inbuf[i++] = ch;
			} else {
				int idx = base64_index(ch, b64_table);
				if (idx == -1) {
					fprintf(stderr, "Invalid base64 character: '%c'\n", ch);
					return 1;
				}
				inbuf[i++] = ch;
			}
		}

		if (i == 0) break; // nothing read, done
		if (i != 4) {
			fprintf(stderr, "Incomplete base64 block\n");
			return 1;
		}

		// Validate padding placement
		if (inbuf[0] == '=' || inbuf[1] == '=') {
			fprintf(stderr, "Invalid padding position in base64 block\n");
			return 1;
		}

		for (int j = 0; j < 4; ++j)
			val[j] = (inbuf[j] == '=') ? 0 : base64_index(inbuf[j], b64_table);

		outbuf[0] = (val[0] << 2) | (val[1] >> 4);
		outbuf[1] = ((val[1] & 0x0F) << 4) | (val[2] >> 2);
		outbuf[2] = ((val[2] & 0x03) << 6) | val[3];

		if (inbuf[2] == '=' && inbuf[3] != '=') {
			fprintf(stderr, "Invalid padding sequence (only last two chars can be '=')\n");
			return 1;
		}

		if (inbuf[2] == '=')
			bytes = 1;
		else if (inbuf[3] == '=')
			bytes = 2;
		else
			bytes = 3;

		fwrite(outbuf, 1, bytes, out);
	}

	return 0;
}

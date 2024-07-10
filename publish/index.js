import { execFile } from "child_process";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

export default function convertDdsToPng(ddsPath, outputPath) {

    const __dirname = dirname(fileURLToPath(import.meta.url));
    const exe = join(__dirname, 'bin', 'DDS-Converter.exe');

    return new Promise((resolve, reject) => {
        let stdout = '';
        let stderr = '';

        const process = execFile(exe, [ddsPath, outputPath]);

        process.stdout.on('data', (data) => {
            stdout += data;
        });

        process.stderr.on('data', (data) => {
            stderr += data;
        });

        process.on('close', (code) => {
            if (code === 0) {
                resolve(stdout);
            } else {
                reject(new Error(`Process exited with code ${code}: ${stderr}`));
            }
        });

        process.on('error', (err) => {
            reject(err);
        });
    });

}
import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
	insecureSkipTLSVerify: true,
	noConnectionReuse: false,
	vus: 400, // number of virtual users
	duration: '1m', // how long test should run
};

const API_BASE_URL = "https://localhost:8888/api/demo/file";

export default function () {

	http.get(`${API_BASE_URL}/integration-test-file.pdf`);
	http.get(`${API_BASE_URL}/big-file.pdf`);

	sleep(1); // this way we will know how many request we can handle per second.
};

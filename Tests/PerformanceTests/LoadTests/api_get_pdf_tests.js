/*
	Load Testing is primarily concerned with assessing the current performance of
	your system in terms of concurrent users or requests per second.
	When you want to understand if your system is meeting the performance goals,
	this is the type of test you will run. 

	Run a load test to:
	- Assess the current performance of your system under typical and peak load
	- Make sure you are continuously meeting the performance standards as you make changes to your system 

	Can be used to simulate a normal day in your business 
*/

import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
	insecureSkipTLSVerify: true,
	noConnectionReuse: false,
	stages: [
		{ duration: '10s', target: 200 }, // simulate ramp-up of traffic
		{ duration: '1m', target: 500 }, // stay at same level of traffic
		{ duration: '1m', target: 700 }, // stay at same level of traffic
		{ duration: '1m', target: 0 }, // ramp-down to 0 users
	]
	,
	thresholds: {
		http_req_duration: ['p(99)<300'] // 99% of request must complete below 300ms.
	}
};

const API_BASE_URL = "https://localhost:8888/api/demo/file";

export default function () {

	http.batch([
		["GET", `${API_BASE_URL}/big-file.pdf`]
		["GET", `${API_BASE_URL}/integration-test-file.pdf`]
	]);

	sleep(1); // this way we will know how many request we can handle per second.
};

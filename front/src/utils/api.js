import { BASE_URL, GET_LINKS_BY_IDS, GET_USER_LINKS, SYNC_LINKS } from "./config";
import { isAuthenticated, authToken } from "../auth0/auth0.store";
import { get } from "svelte/store";

export function buildHeader() {
	let header = {
		"Content-Type": "application/json",
	};

	if (get(isAuthenticated)) {
		header.Authorization = `Bearer ${get(authToken)}`;
	}

	return header;
}

export async function createLink(longURL) {
	const data = {
		url: longURL,
	};

	return await fetch(BASE_URL, {
		method: "POST",
		headers: buildHeader(),
		body: JSON.stringify(data),
	});
}

export async function getUserLinks() {
	return await fetch(GET_USER_LINKS, {
		method: "GET",
		headers: buildHeader(),
	});
}

export async function getLinks(links) {
	var url = new URL(GET_LINKS_BY_IDS);
	links.forEach((link) => url.searchParams.append("ids", link.id));

	return await fetch(url, {
		method: "GET",
		headers: buildHeader(),
	});
}

export async function syncLinksWithUser(links) {
	return await fetch(SYNC_LINKS, {
		method: "PUT",
		headers: buildHeader(),
		body: JSON.stringify(links.map((l) => l.id)),
	});
}

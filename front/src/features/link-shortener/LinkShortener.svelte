<script lang="ts">
  import { onMount } from "svelte";
  import {
    validURL,
    getSourceURLFromParameter,
    clearLinkParameter,
  } from "../../utils/url";
  import { isEnterKeyDown } from "../../utils/keyboard";
  import { gaEventUserCreateShortLink } from "../../utils/ga.js";
  import {
    NAME,
    TITLE,
    SHORT,
    PASTE_LONG_URL,
    INTERNET_CONNECTION,
    URL_INVALID,
    URL_MANDATORY,
  } from "../../utils/resources.js";
  import Message from "../../components/Message.svelte";

  import { createNewLink } from "../../services/link/link-service";

  let longInputElement: HTMLElement;
  let sourceURL: string = null;
  let message: string = null;

  const createShortURL = async () => {
    if (!sourceURL) {
      message = URL_MANDATORY;
      return;
    }

    if (!validURL(sourceURL)) {
      message = URL_INVALID;
      return;
    }

    try {
      gaEventUserCreateShortLink();

      const response = await createNewLink(sourceURL);

      if (response.ok) {
        sourceURL = null;
      } else {
        const data = await response.json();
        message = data.error;
      }
    } catch (exception) {
      message = INTERNET_CONNECTION;
    }
  };

  onMount(() => {
    sourceURL = getSourceURLFromParameter();

    if (sourceURL) {
      clearLinkParameter();
      createShortURL();
    } else {
      longInputElement.focus();
    }
  });
</script>

<section class="container medium">
  <h2>{NAME}</h2>
  <p>{TITLE}</p>
  <div class="row">
    <div class="col-12 col-12-mobilep">
      <input
        type="text"
        autocomplete="false"
        bind:value={sourceURL}
        placeholder={PASTE_LONG_URL}
        bind:this={longInputElement}
        on:keydown={(event) => isEnterKeyDown(event) && createShortURL()}
      />
    </div>
    <Message bind:message />
  </div>
  <div class="row">
    <div class="col-12 col-12-mobilep">
      <button on:click={createShortURL}>{SHORT}</button>
    </div>
  </div>
</section>

<style>
  section {
    color: #fff;
    padding: 12em 0 1em 0;
    text-align: center;
  }

  section :last-child {
    margin-bottom: 10px;
  }

  h2 {
    color: #fff;
  }

  section h2 {
    font-size: 3.5em;
    line-height: 1em;
    margin: 0 0 0.5em 0;
    padding: 0;
  }

  section p {
    font-size: 1.25em;
    margin-bottom: 1.75em;
  }

  section button {
    background-color: transparent;
    box-shadow: inset 0 0 0 2px rgba(255, 255, 255, 0.5);
    color: #fff;
    min-width: 12em;
  }

  section button:hover {
    background-color: rgba(255, 255, 255, 0.1);
  }

  section button:active {
    background-color: rgba(255, 255, 255, 0.2);
  }

  @media screen and (max-width: 1280px) {
    section {
      background-attachment: scroll;
    }

    section h2 {
      font-size: 3.5em;
    }
  }

  @media screen and (max-width: 736px) {
    section {
      padding: 4em 0;
    }

    section h2 {
      font-size: 2.25em;
    }

    section p {
      font-size: 1.25em;
    }
  }

  @media screen and (max-width: 480px) {
    section {
      padding: 5em 1em 0 1em;
    }
  }
</style>

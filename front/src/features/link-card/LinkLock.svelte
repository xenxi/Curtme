<script lang="ts">
  import { tick } from "svelte";
  import { lockLink } from "../../services/api-service";
  import {
    INTERNET_CONNECTION,
    LINK_CUSTOMIZED,
    SET_PASSWORD_PLACEHOLDER,
  } from "../../utils/resources";
  import { isEnterKeyDown } from "../../utils/keyboard";
  import type { LinkModel } from "../../model/link-model";
  import { currentAction, ACTION } from "./link.store";
  import { useMessage } from "../../utils/use-event";

  export let link: LinkModel = null;
  let passwordInput: HTMLElement = null;
  let newPassword: string = null;
  let isModifyingPassword: boolean = false;

  const dispatchMessage = useMessage();

  currentAction.subscribe((newMode) => {
    if (isModifyingPassword && newMode !== ACTION.PASSWORD) {
      cleanUpPasswordVariables();
    }
  });

  const setNewPassword = async () => {
    currentAction.set(ACTION.PASSWORD);
    isModifyingPassword = true;
    newPassword = null;
    await tick();
    passwordInput.focus();
  };

  const saveNewPassword = async () => {
    try {
      const response = await lockLink(link.id, newPassword);
      if (response.ok) {
        cleanUpPasswordVariables();
        dispatchMessage(LINK_CUSTOMIZED);
        link = await response.json();
      } else {
        const data = await response.json();
        dispatchMessage(data.error);
      }
    } catch (error) {
      dispatchMessage(INTERNET_CONNECTION);
    }
  };

  const cleanUpPasswordVariables = () => {
    currentAction.set(ACTION.NONE);
    isModifyingPassword = false;
    newPassword = null;
  };
</script>

{#if isModifyingPassword}
  <input
    type="text"
    class="small-input"
    bind:value={newPassword}
    bind:this={passwordInput}
    placeholder={SET_PASSWORD_PLACEHOLDER}
    on:keydown={(event) => isEnterKeyDown(event) && saveNewPassword()}
  />
  <button class="icon" on:click={saveNewPassword} alt="Save">
    <i class="fa fa-save" />
  </button>
  <button class="icon" on:click={cleanUpPasswordVariables} alt="Cancel">
    <i class="fa fa-times-circle" />
  </button>
{:else}
  <button class="icon" on:click={setNewPassword} alt="Customise Password">
    <i class={link.hasPassword ? "fa fa-lock" : "fa fa-lock-open"} />
  </button>
{/if}

<style>
  button {
    background-color: transparent;
    box-shadow: unset !important;
    color: #e89980;
    padding-left: 15px !important;
  }

  button:disabled {
    color: lightgray;
    pointer-events: none;
  }

  button:hover {
    color: #ff6232;
  }

  input[type="text"] {
    height: unset !important;
    line-height: normal !important;
    margin-right: 5px;
    width: 80%;
  }

  .small-input {
    width: 30% !important;
  }
</style>

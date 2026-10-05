<style>
	details.clean-accordion {
		background-color: #fff; /* Changed to white */
		border: 1px solid #ddd;
		border-radius: 5px;
		margin-bottom: 0.5em;
		overflow: hidden;
	}
	details.clean-accordion summary {
		font-weight: 600;
		padding: 0.6em 1em; /* Reduced padding */
		cursor: pointer;
		background-color: #f5f5f5;
		border-bottom: 1px solid #ddd;
		transition: background-color 0.2s;
		list-style: none;
		display: block;
	}
	details.clean-accordion summary::-webkit-details-marker {
		display: none;
	}
	details.clean-accordion[open] > summary {
		background-color: #e9e9e9;
	}
	details.clean-accordion summary:hover {
		background-color: #e1e1e1;
	}
	details.clean-accordion .token-details {
		padding: 0.8em 1em 1em 2em; /* Reduced padding, kept indent */
		font-size: 0.9em;
	}
	details.clean-accordion .token-details p {
		margin-top: 0;
	}
	details.clean-accordion .token-details pre {
		white-space: pre-wrap;
	}
</style>
<details class="clean-accordion">
	<summary>AssignDataModel</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Assigns and prepares the data model for Razor event templates. It calls the base class&#39;s AssignDataModel, sets up date parameters for calendar views, and populates several event lists (next events, past events, monthly events, etc.) into the SimplisityRazor model.</p>
		<strong>Signature</strong>
		<pre><code>public new string AssignDataModel(SimplisityRazor sModel)</code></pre>
		<strong>Example</strong>
		<pre><code>@AssignDataModel(sModel)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RssEventUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a URL for an RSS feed of events for a specific month and year. The URL includes parameters for the command, month, year, and a SQL index for sorting by event start date.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RssEventUrl(int portalId, string cmd, int monthDate, int yearDate)</code></pre>
		<strong>Example</strong>
		<pre><code>@RssEventUrl(portalId, cmd, monthDate, yearDate)</code></pre>
	</div>
</details>

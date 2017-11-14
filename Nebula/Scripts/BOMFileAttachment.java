package mytest;

import java.io.*;
import java.net.*;
import java.nio.file.Files;
import java.nio.file.LinkOption;
import java.sql.Timestamp;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.Map.Entry;


import com.agile.api.*;


class MyLog
{
	public void info(String info)
	{
		Timestamp ts = new Timestamp(System.currentTimeMillis());   
        DateFormat sdf = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        String tsStr = sdf.format(ts);    
		System.out.println(tsStr +" : "+ info);
	}
	
	public void debug(String info)
	{
		Timestamp ts = new Timestamp(System.currentTimeMillis());   
        DateFormat sdf = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        String tsStr = sdf.format(ts);    
		System.out.println(tsStr +" : "+ info);
	}
	
	public void error(String info)
	{
		Timestamp ts = new Timestamp(System.currentTimeMillis());   
        DateFormat sdf = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        String tsStr = sdf.format(ts);    
		System.out.println(tsStr +" : "+ info);
	}
	
}

class WorkFlowTable
{
	public WorkFlowTable()
	{
		StatusCode = "";
		WorkFlow = "";
		WorkFlowStatus = "";
		Action = "";
		Reqd = "";
		Reviewer = "";
		SignoffUser = "";
		StatusChangedBy = "";
		LocalTime = "";
		SignoffComment = "";
		SignoffDuration = "";
	}
	
	public void SetValue(int key,String value)
	{
		if(value == null)
			return;
		
		if(key == ChangeConstants.ATT_WORKFLOW_STATUS_CODE) StatusCode = value;
		if(key == ChangeConstants.ATT_WORKFLOW_WORKFLOW) WorkFlow = value;
		if(key == ChangeConstants.ATT_WORKFLOW_WORKFLOW_STATUS) WorkFlowStatus = value;
		if(key == ChangeConstants.ATT_WORKFLOW_ACTION) Action = value;
		if(key == ChangeConstants.ATT_WORKFLOW_REQ_D) Reqd = value;
		if(key == ChangeConstants.ATT_WORKFLOW_REVIEWER) Reviewer = value;
		if(key == ChangeConstants.ATT_WORKFLOW_SIGNOFF_USER) SignoffUser = value;
		if(key == ChangeConstants.ATT_WORKFLOW_STATUS_CHANGED_BY) StatusChangedBy = value;
		if(key == ChangeConstants.ATT_WORKFLOW_LOCAL_CLIENT_TIME) LocalTime = value;
		if(key == ChangeConstants.ATT_WORKFLOW_SIGNOFF_COMMENTS) SignoffComment = value;
		if(key == ChangeConstants.ATT_WORKFLOW_SIGNOFF_DURATION) SignoffDuration = value;
	}
	
	public String StatusCode;
	public String WorkFlow;
	public String WorkFlowStatus;
	public String Action;
	public String Reqd;
	public String Reviewer;
	public String SignoffUser;
	public String StatusChangedBy;
	public String LocalTime;
	public String SignoffComment;
	public String SignoffDuration;
	
	public void ExportBR(DataOutputStream out) throws IOException
	{
		out.writeBytes("<<WORKFLOW>> StatusCode:::"+StatusCode+"###"+"WorkFlow:::"+WorkFlow+"###"+"WorkFlowStatus:::"+WorkFlowStatus+"###"+"Action:::"+Action
				+"###"+"Reqd:::"+Reqd+"###"+"Reviewer:::"+Reviewer+"###"+"SignoffUser:::"+SignoffUser+"###"+"StatusChangedBy:::"+StatusChangedBy
				+"###"+"LocalTime:::"+LocalTime+"###"+"SignoffComment:::"+SignoffComment+"###"+"SignoffDuration:::"+SignoffDuration+"\r\n");
	}
			
}

class BRBaseInfo
{
	public BRBaseInfo()
	{
		ChangeType = "";
		Number = "";
		Description = "";
		Status = "";
		Workflow = "";
		Originator = "";
		OriginalDate = "";
		brworkflowlist = null;
		affectitem = null;
		history = null;
		attach = null;
	}
	
	public void SetValue(int key,String value)
	{
		if(value == null)
			return;
		
		if(key == ChangeConstants.ATT_COVER_PAGE_CHANGE_TYPE) ChangeType = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_NUMBER) Number = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_DESCRIPTION) Description = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_DESCRIPTION_OF_CHANGE) Description = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_STATUS) Status = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_WORKFLOW) Workflow = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_ORIGINATOR) Originator = value;
		if(key == ChangeConstants.ATT_COVER_PAGE_DATE_ORIGINATED) OriginalDate = value;
	}
	
	public String ChangeType;
	public String Number;
	public String Description;
	public String Status;
	public String Workflow;
	public String Originator;
	public String OriginalDate;
	
	List<WorkFlowTable> brworkflowlist;
	List<BRAffectItem> affectitem;
	List<BRHistory> history;
	List<BRAttachment> attach;
	
	private static boolean CreateDir(String dirstr)
	{
		File dir = new File(dirstr);
		if(!dir.exists())
		{
			try
			{
				dir.mkdirs();
				return true;
			}
			catch(Exception ex)
			{
				return false;
			}
		}
		
		return true;
	}
	
	public static String getCurrentTimeStamp() {
	    SimpleDateFormat sdfDate = new SimpleDateFormat("yyyy-MM-dd");
	    Date now = new Date();
	    String strDate = sdfDate.format(now);
	    return strDate;
	}
	
	public void ExportBR(String AgileDir)
	{
		if(!Number.isEmpty())
		{
			String dir = AgileDir+Number;
			if(CreateDir(dir))
			{
				String brfilename = dir+"/"+Number+"-"+getCurrentTimeStamp()+".txt";
				File f=new File(brfilename);
				DataOutputStream out=null;
				try {
					out = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(brfilename)));
					out.writeBytes("<<BASEINFO>> Number:::"+Number+"###"+"ChangeType:::"+ChangeType+"###"+"Status:::"+Status
							+"###"+"Workflow:::"+Workflow+"###"+"Originator:::"+Originator+"###"+"OriginalDate:::"+OriginalDate+"###"+"Description:::"+Description+"\r\n");
					if(brworkflowlist != null && brworkflowlist.size() > 0)
					{
						for(int idx = 0;idx < brworkflowlist.size();idx++)
						{
							brworkflowlist.get(idx).ExportBR(out);
						}
					}
					if(affectitem != null && affectitem.size() > 0)
					{
						for(int idx = 0;idx < affectitem.size();idx++)
						{
							affectitem.get(idx).ExportBR(out);
						}
					}
					if(history != null && history.size() > 0)
					{
						for(int idx = 0;idx < history.size();idx++)
						{
							history.get(idx).ExportBR(out);
						}
					}
					if(attach != null && attach.size() > 0)
					{
						for(int idx = 0;idx < attach.size();idx++)
						{
							attach.get(idx).ExportBR(out);
						}
					}
					out.close();
				}catch(Exception ex)
				{
					System.out.println(ex.getMessage());
				}
				
			}//end if
		}//end if
	}
	
}

class BRAffectItem
{
	public BRAffectItem()
	{
		itemnumber = "";
		itemsite = "";
		itemdesc = "";
		lifecycle = "";
		commodity = "";
	}
	
	public void SetValue(int key,String value)
	{
		if(value == null)
			return;
		
		if(key == ChangeConstants.ATT_AFFECTED_ITEMS_ITEM_NUMBER) itemnumber = value;
		if(key == ChangeConstants.ATT_AFFECTED_ITEMS_SITES) itemsite = value;
		if(key == ChangeConstants.ATT_AFFECTED_ITEMS_ITEM_DESCRIPTION) itemdesc = value;
		if(key == ChangeConstants.ATT_AFFECTED_ITEMS_LIFECYCLE_PHASE) lifecycle = value;
		if(key == ChangeConstants.ATT_AFFECTED_ITEMS_COMMODITY) commodity = value;
	}
	
	public String itemnumber;
	public String itemsite;
	public String itemdesc;
	public String lifecycle;
	public String commodity;
	
	public void ExportBR(DataOutputStream out) throws IOException
	{
		out.writeBytes("<<AFFECT>> itemnumber:::"+itemnumber+"###"+"itemsite:::"+itemsite+"###"+"itemdesc:::"+itemdesc+"###"+"lifecycle:::"+lifecycle+"###"+"commodity:::"+commodity+"\r\n");
	}
}

class BRAttachment
{
	public BRAttachment()
	{
		FileName = "";
		ModifyDate = "";
		Checkiner = "";
		LocalFilePath = "";
	}
	
	public void SetValue(int key,String value)
	{
		if(value == null)
			return;
		
		if(key == ChangeConstants.ATT_ATTACHMENTS_FILE_NAME) FileName = value;
		if(key == ChangeConstants.ATT_ATTACHMENTS_MODIFIED_DATE) ModifyDate = value;
		if(key == ChangeConstants.ATT_ATTACHMENTS_CHECKIN_USER) Checkiner = value;
	}
	
	public String FileName;
	public String ModifyDate;
	public String Checkiner;
	public String LocalFilePath;
	
	public void ExportBR(DataOutputStream out) throws IOException
	{
		out.writeBytes("<<ATTACH>> FileName:::"+FileName+"###"+"ModifyDate:::"+ModifyDate+"###"+"Checkiner:::"+Checkiner+"###"+"LocalFilePath:::"+LocalFilePath+"\r\n");
	}
}

class BRHistory
{
	public BRHistory()
	{
		status = "";
		nextstatus = "";
		action = "";
		user = "";
		localtime = "";
		detail = "";
		usernotice = "";
	}
	
	public void SetValue(int key,String value)
	{
		if(value == null)
			return;
		
		if(key == ChangeConstants.ATT_HISTORY_STATUS) status = value;
		if(key == ChangeConstants.ATT_HISTORY_NEXT_STATUS) nextstatus = value;
		if(key == ChangeConstants.ATT_HISTORY_ACTION) action = value;
		if(key == ChangeConstants.ATT_HISTORY_USER) user = value;
		if(key == ChangeConstants.ATT_HISTORY_LOCAL_CLIENT_TIME) localtime = value;
		if(key == ChangeConstants.ATT_HISTORY_DETAILS) detail = value;
		if(key == ChangeConstants.ATT_HISTORY_USERS_NOTIFIED) usernotice = value;
	}
	
	public String status;
	public String nextstatus;
	public String action;
	public String user;
	public String localtime;
	public String detail;
	public String usernotice;
	
	public void ExportBR(DataOutputStream out) throws IOException
	{
		out.writeBytes("<<HISTORY>> status:::"+status+"###"+"nextstatus:::"+nextstatus+"###"+"action:::"+action+"###"+"user:::"+user
				+"###"+"localtime:::"+localtime+"###"+"detail:::"+detail+"###"+"usernotice:::"+usernotice+"\r\n");
	}
}


public class BOMFileAttachment {

	public static final MyLog goLogger =new MyLog();//Logger.getLogger(BOMFileAttachment.class);

   // main method.. 
	public static void main(String[] args)
	{
		BRFromAgile(args);
		ECOFromAgile(args);
	}
	    
	
	private static void NewBRQuery(String[] args,String AgileURL,String AgileDir,String LocalSitePort)
	{
		goLogger.info("get AgileURL "+AgileURL);
		
		IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
		if(sess != null)
		{
			List<String> brstrlist = new ArrayList<String>();
			
			for(int aidx = 4;aidx < args.length;aidx++)
			{
				String pmnametime = args[aidx];
				String pmname = pmnametime.split(";;;")[0].replace("###", " ");
				String latesttime = pmnametime.split(";;;")[1].replace("###", " ");
				
				if(sess == null)
				{
					sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
					if(sess == null)
						return;
				}
				List<BRBaseInfo> brlist =  null;
				try
				{
					BOMFileAttachment gfa=new BOMFileAttachment();
					sess.setDateFormats(new DateFormat[]{new SimpleDateFormat("yyyy-MM-dd hh:mm:ss")});
					brlist =  gfa.RetrieveBRBaseInfo(pmname,latesttime,sess);
					
					for(int idx = 0;idx < brlist.size();idx++)
					{
						BRBaseInfo brinfo = brlist.get(idx);
						IChange BR = (IChange)sess.getObject(IChange.OBJECT_TYPE, brinfo.Number);
						if(BR != null)
						{
							brinfo.brworkflowlist = gfa.RetrieveBRWorkFlow(BR);
							brinfo.affectitem =  gfa.RetrieveBRAffectItem(BR);
							brinfo.history = gfa.RetrieveBRHistory(BR);
							brinfo.attach = gfa.RetrieveBRAttachment(BR,AgileDir+brinfo.Number+"/");
						}
					}
					
				}
				catch(Exception ex)
				{
					System.out.println(ex.getMessage());
					sess.close();
					sess = null;
				}
				

				if(brlist != null)
				{
					try
					{
						for(int idx=0;idx < brlist.size();idx++)
						{
							brlist.get(idx).ExportBR(AgileDir);
							brstrlist.add(brlist.get(idx).Number);
						}
					}catch(Exception ex){}
				}


			}//end for
			
			if(sess != null)
			{
				sess.close();
			}
			
			NoticNebulaNewBR(LocalSitePort,brstrlist);
		}//end if
	}
	
	private static void UpdateBRList(String[] args,String AgileURL,String AgileDir,String LocalSitePort)
	{
		IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
		if(sess != null)
		{
			List<BRBaseInfo> brlist =  new ArrayList<BRBaseInfo>();
			if(args.length > 4)
			{
				for(int aidx = 4;aidx < args.length;aidx++)
				{
					String brstr = args[aidx];
					
					if(sess == null)
					{
						sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
						if(sess == null)
							return;
					}
					try
					{
						BRBaseInfo brinfo = new BRBaseInfo();
						brinfo.Number = brstr;
						
						BOMFileAttachment gfa=new BOMFileAttachment();
						sess.setDateFormats(new DateFormat[]{new SimpleDateFormat("yyyy-MM-dd hh:mm:ss")});
						
						IChange BR = (IChange)sess.getObject(IChange.OBJECT_TYPE, brstr);
						if(BR != null)
						{
							brinfo.brworkflowlist = gfa.RetrieveBRWorkFlow(BR);
							brinfo.affectitem =  gfa.RetrieveBRAffectItem(BR);
							brinfo.history = gfa.RetrieveBRHistory(BR);
							brinfo.attach = gfa.RetrieveBRAttachment(BR,AgileDir+brinfo.Number+"/");
							
							brlist.add(brinfo);
						}
					}
					catch(Exception ex)
					{
						System.out.println(ex.getMessage());
						sess.close();
						sess = null;
					}
				}//end for				
			}
			
			if(brlist.size() > 0)
			{
				try
				{
					for(int idx=0;idx < brlist.size();idx++)
					{
						brlist.get(idx).ExportBR(AgileDir);
					}
				}catch(Exception ex){}
			}
			
			if(sess != null)
			{
				sess.close();
			}
			
			NoticNebulaUpdateBR(LocalSitePort);
		}//end if
	}
	
	//http://sny-agile9app-p64:7001/Agile
	private static void BRFromAgile(String[] args)
	{
		if(args.length > 3)
		{
			
			for(int idx = 0;idx < args.length;idx++)
			{
				goLogger.info("java main...param "+idx+" is "+args[idx]);				
			}
			String Function = args[0];
			String AgileURL = args[1];
			String LocalSitePort = args[2];
			String AgileDir = args[3];
			goLogger.info("Function  is "+Function);
			
			if(Function.equalsIgnoreCase("UPDATEBRLIST"))
			{
				goLogger.info("run update br list");
				UpdateBRList(args, AgileURL, AgileDir, LocalSitePort);
			}
			else if(Function.equalsIgnoreCase("NEWBRQUERY"))
			{
				goLogger.info("run new query");
				NewBRQuery(args, AgileURL, AgileDir, LocalSitePort);
			}
		}//end if
		else
		{
			goLogger.error("\n Usage: UPDATEBRLIST/NEWBRQUERY  AgileURL LocalSitePort AgileDir User###Name1;;;TIME1 User###Name2;;;TIME2 User###Name3;;;TIME3.....");	
		}
	}
	
	private static void NoticNebulaNewBR(String LocalSitePort,List<String> brlist)
	{
			String brstr = "";

			for(int idx = 0;idx < brlist.size();idx++)
			{
				brstr = brstr + ":::"+ brlist.get(idx);
			}
			
			try
			{
				goLogger.info( "try to query: "+"http://localhost:"+LocalSitePort+"/BRTrace/NewBR?BRLIST="+brstr);
				
				URL url = new URL("http://localhost:"+LocalSitePort+"/BRTrace/NewBR?BRLIST="+brstr);
		        URLConnection URLconnection = url.openConnection();  
		        HttpURLConnection httpConnection = (HttpURLConnection)URLconnection;  
		        int responseCode = httpConnection.getResponseCode();
		        if (responseCode == HttpURLConnection.HTTP_OK) {  
		        	goLogger.info("Query HTTP server successfully");
		        	
		        	InputStream urlStream = httpConnection.getInputStream();  
		            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(urlStream));  
		            String sCurrentLine = "";  
		            while ((sCurrentLine = bufferedReader.readLine()) != null) {  
		            	goLogger.info( sCurrentLine);
		            }  
		        }
		        else
		        {
		        	goLogger.error("Fail to access url:"+"http://localhost:"+LocalSitePort+"/BRTrace/NewBR?BRLIST="+brstr);
		        }
			}catch(Exception ex)
			{
				goLogger.error("Fail to query url:"+ex.getMessage());			
			}	
	}
	
	private static void NoticNebulaUpdateBR(String LocalSitePort)
	{
			try
			{
				goLogger.info( "try to query: "+"http://localhost:"+LocalSitePort+"/BRTrace/UpdateBR");
				
				URL url = new URL("http://localhost:"+LocalSitePort+"/BRTrace/UpdateBR");
		        URLConnection URLconnection = url.openConnection();  
		        HttpURLConnection httpConnection = (HttpURLConnection)URLconnection;  
		        int responseCode = httpConnection.getResponseCode();
		        if (responseCode == HttpURLConnection.HTTP_OK) {  
		        	goLogger.info("Query HTTP server successfully");
		        	
		        	InputStream urlStream = httpConnection.getInputStream();  
		            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(urlStream));  
		            String sCurrentLine = "";  
		            while ((sCurrentLine = bufferedReader.readLine()) != null) {  
		            	goLogger.info( sCurrentLine);
		            }  
		        }
		        else
		        {
		        	goLogger.error("Fail to access url:"+"http://localhost:"+LocalSitePort+"/BRTrace/UpdateBR");
		        }
			}catch(Exception ex)
			{
				goLogger.error("Fail to query url:"+ex.getMessage());			
			}	
	}
	
	
	private static void ECOFromAgile(String[] args)
	{
		
		/*		if(args.length > 4)
		{
			for(int idx = 0;idx < args.length;idx++)
			{
				goLogger.info("java main...param "+idx+" is "+args[idx]);				
			}
			
			String Mode = args[0];
			String AgileURL = args[1];
			String LocalSitePort = args[2];
			String AgileDir = args[3];
			
			List<String> ecolist = new ArrayList<String>();
			for(int idx = 4;idx < args.length;idx++)
			{
				ecolist.add(args[idx]);
			}
			
			HashMap<String,HashMap<String,String>> localfiledict = null; //RetrieveLocalFiles(AgileDir);
			
			for(int idx = 0;idx < ecolist.size();idx++)
			{
				boolean ret = CreateDir(AgileDir+"\\"+ecolist.get(idx));
				if(!ret)
					return;
			}

			if(Mode.equalsIgnoreCase("ATTACH"))
			{
				IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
				if(sess != null)
				{
					BOMFileAttachment gfa=new BOMFileAttachment();
					gfa.getAgileFilesByName(sess,ecolist,AgileDir,localfiledict,false);
					
					NoticDominoAttach(LocalSitePort,ecolist,"AgileAttach");
					NoticDominoAttach(LocalSitePort,ecolist,"AgileAttach");
					sess.close();
				}
				
				//gfa.getAgileFilesByName("E150570");
				//gfa.getAgileFilesByName("WI-MFG-318");
				//gfa.getAgileFilesByName("38200039");
			}
			if(Mode.equalsIgnoreCase("ATTACHNAME"))
			{
				IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
				if(sess != null)
				{
					BOMFileAttachment gfa=new BOMFileAttachment();
					gfa.getAgileFilesByName(sess,ecolist,AgileDir,localfiledict,true);
					sess.close();
					
					NoticDominoAttach(LocalSitePort,ecolist,"AgileAttach");
					NoticDominoAttach(LocalSitePort,ecolist,"AgileAttach");
					
				}
				
			}
			else if(Mode.equalsIgnoreCase("WORKFLOW"))
			{
				IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
				if(sess != null)
				{
					BOMFileAttachment gfa=new BOMFileAttachment();
					gfa.getAgileWorkFlow(sess,ecolist,AgileDir);
					sess.close();
					
					NoticDominoAttach(LocalSitePort,ecolist,"AgileWorkFlow");
					NoticDominoAttach(LocalSitePort,ecolist,"AgileWorkFlow");
				}
			}
		}
		else
		{
			goLogger.error("\n Usage: ATTACH/WORKFLOW/ATTACHNAME AgileURL LocalSitePort AgileDir ECONUM1 ECONUM2 ECONUM3 .....");
			return;
		}*/
		
	}
	
	
	
	public List<BRBaseInfo> RetrieveBRBaseInfo(String pmname,String starttime,IAgileSession sess)
	{
		List<BRBaseInfo> ret = new ArrayList<BRBaseInfo>();
		
		try
		{
			IQuery query = (IQuery)sess.createObject(IQuery.OBJECT_TYPE,ChangeConstants.CLASS_CHANGE_BASE_CLASS);
			query.setCaseSensitive(false);
			query.setCriteria("[Cover Page.Originator] == '"+pmname+"' and " 
					+"[Cover Page.Change Type] == 'Build Requests' and "
					+"[Cover Page.Date Originated] > '"+starttime+"' ");
			
			Integer resatt [] = {ChangeConstants.ATT_COVER_PAGE_CHANGE_TYPE,ChangeConstants.ATT_COVER_PAGE_NUMBER
					,ChangeConstants.ATT_COVER_PAGE_DESCRIPTION
					,ChangeConstants.ATT_COVER_PAGE_STATUS,ChangeConstants.ATT_COVER_PAGE_WORKFLOW
					,ChangeConstants.ATT_COVER_PAGE_ORIGINATOR,ChangeConstants.ATT_COVER_PAGE_DATE_ORIGINATED};
			query.setResultAttributes(resatt);
			
			ITable tb = query.execute();
			Iterator ite=tb.iterator();
			while(ite.hasNext()){
				IRow row=(IRow) ite.next();
				BRBaseInfo brinfo = new BRBaseInfo();
				
				Map<Integer,Object> mvalues = row.getValues();
				goLogger.debug(mvalues.toString());
				for(Entry<Integer,Object> e: mvalues.entrySet())
				{
					int key = e.getKey();
					if(e.getValue() != null)
					{
						String value = e.getValue().toString();
						brinfo.SetValue(key, value.replace("'", "").replace(",", "").replace("\r", "").replace("\n", ""));
					}
				}
				
				ret.add(brinfo);
			}
		}catch(Exception ex)
		{
			System.out.println(ex.getMessage());
			ret.clear();
		}
		return ret;
	}
	
	public List<WorkFlowTable> RetrieveBRWorkFlow(IChange BR)
	{
		List<WorkFlowTable> ret = new ArrayList<WorkFlowTable>();
		try
		{
			ITable workflowtable = BR.getTable(ChangeConstants.TABLE_WORKFLOW);
			if(workflowtable != null)
			{
				Iterator ite=workflowtable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					WorkFlowTable wtabrow = new WorkFlowTable();
					Map<Integer,Object> mvalues = row.getValues();
					
					goLogger.debug(mvalues.toString());
					
					for(Entry<Integer,Object> e: mvalues.entrySet())
					{
						int key = e.getKey();
						if(e.getValue() != null)
						{
							String value = e.getValue().toString();
							wtabrow.SetValue(key, value.replace("'", "").replace(",", "").replace("\r", "").replace("\n", ""));
						}
					}
					ret.add(wtabrow);
				}//end while
			}
		}
		catch(Exception ex)
		{
			System.out.println(ex.getMessage());
			ret.clear();
		}
		
		return ret;
	}
	
	public List<BRAffectItem> RetrieveBRAffectItem(IChange BR)
	{
		List<BRAffectItem> ret = new ArrayList<BRAffectItem>();
		try
		{
			ITable affecttable = BR.getTable(ChangeConstants.TABLE_AFFECTEDITEMS);
			if(affecttable != null)
			{
				Iterator ite=affecttable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					BRAffectItem wtabrow = new BRAffectItem();
					Map<Integer,Object> mvalues = row.getValues();
					
					goLogger.debug(mvalues.toString());
					
					for(Entry<Integer,Object> e: mvalues.entrySet())
					{
						int key = e.getKey();
						if(e.getValue() != null)
						{
							String value = e.getValue().toString();
							wtabrow.SetValue(key, value.replace("'", "").replace(",", "").replace("\r", "").replace("\n", ""));
						}
					}
					ret.add(wtabrow);
				}//end while
			}
		}
		catch(Exception ex)
		{
			System.out.println(ex.getMessage());
			ret.clear();
		}
		
		return ret;
	}
	
	public List<BRHistory> RetrieveBRHistory(IChange BR)
	{
		List<BRHistory> ret = new ArrayList<BRHistory>();
		try
		{
			ITable historytable = BR.getTable(ChangeConstants.TABLE_HISTORY);
			if(historytable != null)
			{
				Iterator ite=historytable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					BRHistory wtabrow = new BRHistory();
					Map<Integer,Object> mvalues = row.getValues();
					
					goLogger.debug(mvalues.toString());
					
					for(Entry<Integer,Object> e: mvalues.entrySet())
					{
						int key = e.getKey();
						if(e.getValue() != null)
						{
							String value = e.getValue().toString();
							wtabrow.SetValue(key, value.replace("'", "").replace(",", "").replace("\r", "").replace("\n", ""));
						}
					}
					ret.add(wtabrow);
				}//end while
			}
		}
		catch(Exception ex)
		{
			System.out.println(ex.getMessage());
			ret.clear();
		}
		
		return ret;
	}
	
	public List<BRAttachment> RetrieveBRAttachment(IChange BR,String savedlocation)
	{
		List<BRAttachment> ret = new ArrayList<BRAttachment>();
		try
		{
			ITable atttable = BR.getTable(ChangeConstants.TABLE_ATTACHMENTS);
			if(atttable != null)
			{
				Iterator ite=atttable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					BRAttachment wtabrow = new BRAttachment();
					Map<Integer,Object> mvalues = row.getValues();
					
					goLogger.debug(mvalues.toString());
					
					for(Entry<Integer,Object> e: mvalues.entrySet())
					{
						int key = e.getKey();
						if(e.getValue() != null)
						{
							String value = e.getValue().toString();
							wtabrow.SetValue(key, value.replace("'", "").replace(",", "").replace("\r", "").replace("\n", ""));
						}
					}
					
					if(!wtabrow.FileName.isEmpty())
					{
						try
						{
							goLogger.debug("start download attachment:"+wtabrow.FileName);
							if(CreateDir(savedlocation))
							{
								wtabrow.LocalFilePath = savedlocation+wtabrow.FileName;
								File localfile = new File(wtabrow.LocalFilePath);
								if(!localfile.exists())
								{
									InputStream is=((IAttachmentFile)row).getFile();
									createFile(is,wtabrow.FileName,savedlocation);
								}
							}
						}catch(Exception ex)
						{
							goLogger.debug("download attachment exception:"+ex.getMessage());
						}
					}
					ret.add(wtabrow);
				}//end while
			}
		}
		catch(Exception ex)
		{
			System.out.println(ex.getMessage());
			ret.clear();
		}
		
		return ret;
	}
	
	public void getAgileFilesByName(IAgileSession sess,List<String> ecolist,String AgileDir,HashMap<String,HashMap<String,String>> localfiledict,boolean justname) {

		boolean success = true;
		for(int idx = 0;idx < ecolist.size();idx++)
		{
			String savedlocation = AgileDir+"\\"+ecolist.get(idx)+"\\";
        	success = getFilesWithECO(sess,ecolist.get(idx),savedlocation,localfiledict,justname);
        	if(success==true)
            	continue;
        	getFilesWithUniqueKey(sess,ecolist.get(idx),savedlocation,localfiledict,justname);
		}
	}
	
	public void getAgileWorkFlow(IAgileSession sess,List<String> ecolist,String AgileDir) {

		for(int idx = 0;idx < ecolist.size();idx++)
		{
			String savedlocation = AgileDir+"\\"+ecolist.get(idx)+"\\";
        	getWorkFlowWithECO(sess,ecolist.get(idx),savedlocation);
		}
	}
	
	private static HashMap<String,HashMap<String,String>> RetrieveLocalFiles(String AgileDir)
	{
		HashMap<String,HashMap<String,String>> ret = new HashMap<String,HashMap<String,String>>();
        File maindir = new File(AgileDir);
        if(!maindir.exists())
        	return ret;
        
        File[] subdirs = maindir.listFiles();
        for (File subdir : subdirs){
        	
            if (subdir.isDirectory())
            {
            	HashMap<String,String> value = new HashMap<String,String>();
            	File[] detailfiles = subdir.listFiles();
            	for(File detailfile : detailfiles)
            	{
            		if(detailfile.isFile())
            		{
            			value.put(detailfile.getName(), "true");
            		}
            	}//end for
            	
            	ret.put(subdir.getName(),value);
            }//end if
        }//end for
		
		return ret;
	}
	
	private static boolean AttachExist(HashMap<String,HashMap<String,String>> localfiledict,String ECONum,String FN)
	{
		HashMap<String,String> ecoexist = localfiledict.get(ECONum);
		if(ecoexist != null)
		{
			String attachexist	= ecoexist.get(FN);
			if(attachexist != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	// Getting Agile Session
	 private static IAgileSession getAgileSession(String agileurl,String user,String password) 
	 {
		//String url="http://sny-agile9app-p64:7001/Agile";
		String url = agileurl;
		String uname=user;
		String upwd=password;
		goLogger.debug("getAgileSession:"+"\nurl="+url + "  username=" + uname + "  pwd=" + upwd);
		HashMap params=new HashMap();
		params.put(AgileSessionFactory.USERNAME, uname);
		params.put(AgileSessionFactory.PASSWORD, upwd);
		
		try 
		{
			AgileSessionFactory fac=AgileSessionFactory.getInstance(url);
			IAgileSession sess =  fac.createSession(params);
			if(sess == null)
			{
				goLogger.debug("Fail to get Agile session" );
			}
			else
			{
				goLogger.debug(" Got Agile Session" );
			}
			return sess;
		}
		catch (APIException e) 
		{
		  goLogger.debug("Error while connecting to Agile\n...Msg="+e.getMessage()+"\nurl="+url );
		}
		
		return null;
	 }
	 
	private static boolean CreateDir(String dirstr)
	{
		File dir = new File(dirstr);
		if(!dir.exists())
		{
			try
			{
				dir.mkdirs();
				return true;
			}
			catch(Exception ex)
			{
				goLogger.error("\n fail to create directory : "+ dirstr);
				return false;
			}
		}
		
		return true;
	}
		
	private static void NoticDominoAttach(String LocalSitePort,List<String> ecolist,String action)
	{
		for(int idx = 0;idx < ecolist.size();idx++)
		{
			String econum = ecolist.get(idx);
			try
			{
				goLogger.info( "try to query: "+"http://localhost:"+LocalSitePort+"/Domino/MiniPIP/"+action+"?ECONUM="+econum);
				
				URL url = new URL("http://localhost:"+LocalSitePort+"/Domino/MiniPIP/"+action+"?ECONUM="+econum);
		        URLConnection URLconnection = url.openConnection();  
		        HttpURLConnection httpConnection = (HttpURLConnection)URLconnection;  
		        int responseCode = httpConnection.getResponseCode();
		        if (responseCode == HttpURLConnection.HTTP_OK) {  
		        	goLogger.info("Query HTTP server successfully");
		        	
		        	InputStream urlStream = httpConnection.getInputStream();  
		            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(urlStream));  
		            String sCurrentLine = "";  
		            while ((sCurrentLine = bufferedReader.readLine()) != null) {  
		            	goLogger.info( sCurrentLine);
		            }  
		        }
		        else
		        {
		        	goLogger.error("Fail to access url:"+"http://localhost:"+LocalSitePort+"/Domino/MiniPIP/"+action+"?ECONUM="+econum);
		        }
			}catch(Exception ex)
			{
				goLogger.error("Fail to query url:"+ex.getMessage());			
			}				
		}
	}
	
	 private boolean scanAttachement(ITable atttable,String savedlocation,String econum,HashMap<String,HashMap<String,String>> localfiledict,boolean justname)
	 {
		 try
		 {
			 boolean ret = true;
			 Iterator ite=atttable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					String attname=row.getValue(ItemConstants.ATT_ATTACHMENTS_FILENAME).toString();
					goLogger.debug("Got attachment... with name "+attname);
//					if(!AttachExist(localfiledict,econum,attname))
//					{
					 if(justname)
					 {
						 File f=new File(savedlocation+attname);
							DataOutputStream out=null;
							try {
									out = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(savedlocation+attname)));
									out.writeBytes("Hello World");
									out.close();
							}catch (FileNotFoundException e) {
								goLogger.error("Exception in getting output stream  file.."+e.getMessage());
							}catch (IOException e) {
								goLogger.error("Exception in writing to file.."+e.getMessage());
							}
					 }
					 else
					 {
						   try{
							   InputStream is=((IAttachmentFile)row).getFile();
							   goLogger.info("start download file "+attname);
							   createFile(is,attname,savedlocation);
						   }
						   catch(Throwable a){
								goLogger.error("Please check if the file server up and running and the specified file downloadable.");
								ret = false;
						   }						 
					 }
						
//					}
				 }
			 return ret;			 
		 }
		 catch (APIException e)
		 {
			 goLogger.error("Exception="+e.getMessage());
			 return false;
		 }
	 }
	
	private boolean getFilesWithUniqueKey(IAgileSession ses,String Bomnumber,String savedlocation,HashMap<String,HashMap<String,String>> localfiledict,boolean justname)
	{
    	try 
    	{
	    	IItem item=(IItem) ses.getObject(ItemConstants.CLASS_ITEM_BASE_CLASS, Bomnumber);
	    	if(item!=null)
	    	{
	    		goLogger.debug("Seraching Attachment...with unique key "+Bomnumber);
				ITable atttable=item.getTable(ItemConstants.TABLE_ATTACHMENTS);//**
				boolean success = scanAttachement(atttable, savedlocation,Bomnumber,localfiledict,justname);
				if(!success)
				{
					goLogger.debug("There is no file with  in the BOM "+Bomnumber);
					return false;
				}
				return true;
	    	}
	    	else
	    	{
	    		goLogger.debug("The unique key  "+Bomnumber+ " is not exist in Agile");
	    	}
		}
    	catch(NullPointerException np){
		    goLogger.error("Null Exception="+np.getMessage());
		}catch (APIException e) {
			goLogger.error("Exception="+e.getMessage());
		}
		return false;
	}
	
	private boolean getFilesWithECO(IAgileSession ses,String Bomnumber,String savedlocation,HashMap<String,HashMap<String,String>> localfiledict,boolean justname) 
	{
    	try 
    	{
	    	IChange eco = (IChange)ses.getObject(IChange.OBJECT_TYPE, Bomnumber);
	    	if(eco == null)
			{
				goLogger.info("Fail to get class IChange from "+Bomnumber);
				return false;
			}
			else
			{
				goLogger.info("get class IChange "+eco.toString()+" "+eco.getName());
				ITable atttable = eco.getAttachments();
				if(atttable == null)
				{
					goLogger.info("Fail to get Attachments from ECO "+Bomnumber);
					return false;
				}
				else
				{
					boolean ret = scanAttachement(atttable, savedlocation,Bomnumber,localfiledict,justname);
					return ret;
				}
			}
		}
    	catch(NullPointerException np){
		    goLogger.error("Null Exception="+np.getMessage());
		    return false;
		}catch (APIException e) {
			goLogger.error("Exception="+e.getMessage());
			return false;
		}
	}
	
	private void getWorkFlowWithECO(IAgileSession ses,String Bomnumber,String savedlocation)
	{
		try 
    	{
			IChange eco = (IChange)ses.getObject(IChange.OBJECT_TYPE, Bomnumber);
	    	if(eco == null)
			{
				goLogger.info("Fail to get class IChange from "+Bomnumber);
				return;
			}
			else
			{
				goLogger.info("get class IChange "+eco.toString()+" "+eco.getName());
				
					ITable workflowtable = eco.getTable(ChangeConstants.TABLE_WORKFLOW);
					if(workflowtable != null)
					{
						List<WorkFlowTable> wtab = new ArrayList<WorkFlowTable>();
						Iterator ite=workflowtable.iterator();
						while(ite.hasNext()){
							IRow row=(IRow) ite.next();
							
							WorkFlowTable wtabrow = new WorkFlowTable();
							
							Map<Integer,Object> mvalues = row.getValues();
							
							goLogger.debug(mvalues.toString());
							
							for(Entry<Integer,Object> e: mvalues.entrySet())
							{
								int key = e.getKey();
								if(e.getValue() != null)
								{
									String value = e.getValue().toString();
									wtabrow.SetValue(key, value.replace("'", "").replace(",", "").replace("\r", "").replace("\n", ""));
								}
							}
							wtab.add(wtabrow);
						}//end while
						
						if(wtab.size() > 0)
						{
							File f=new File(savedlocation+Bomnumber+"_WorkFlowTable.csv");
							DataOutputStream out=null;
							try {
							out = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(savedlocation+Bomnumber+"_WorkFlowTable.csv")));
							out.writeBytes("StatusCode,"+"WorkFlow,"+"WorkFlowStatus,"
									+"Action,"+"Reqd,"+"Reviewer,"
									+"SignoffUser,"+"StatusChangedBy,"+"LocalTime,"
									+"SignoffComment,"+"SignoffDuration\r\n");
							
							for(int idx = 0;idx < wtab.size();idx++)
							{
								out.writeBytes(wtab.get(idx).StatusCode+","+wtab.get(idx).WorkFlow+","+wtab.get(idx).WorkFlowStatus+","+
										wtab.get(idx).Action+","+wtab.get(idx).Reqd+","+wtab.get(idx).Reviewer+","+
										wtab.get(idx).SignoffUser+","+wtab.get(idx).StatusChangedBy+","+wtab.get(idx).LocalTime+","+
										wtab.get(idx).SignoffComment+","+wtab.get(idx).SignoffDuration+"\r\n");
							}
							out.close();
							goLogger.debug("WorkFlow is saved at location: "+f.getAbsolutePath());
							
						}catch (FileNotFoundException e) {
							goLogger.error("Exception in getting output stream  file.."+e.getMessage());
						}catch (IOException e) {
							goLogger.error("Exception in writing to file.."+e.getMessage());
						}
						}//end if
						
					}//end if
			}
		}
    	catch(NullPointerException np){
		    goLogger.error("Null Exception="+np.getMessage());
		    return ;
		}catch (APIException e) {
			goLogger.error("Exception="+e.getMessage());
			return ;
		}
		
	}
	
	private void createFile(InputStream fin,String filename,String loc) {
		goLogger.debug("Copying the attachment..");
		File f=new File(loc+filename);
		DataOutputStream out=null;
		try {
		out = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(loc+filename)));
		}catch (FileNotFoundException e) {
			goLogger.error("Exception in getting output stream  file.."+e.getMessage());
		}
		
		boolean again = true;
		try {
		while(again) {
			int readpart;
			readpart = fin.read();
			if(readpart > -1) {
				out.writeByte(readpart);
			}
			else again = false;
		}
		out.close();
		fin.close();
		} catch (IOException e) {
			goLogger.error("Exception in writing to file.."+e.getMessage());
		}
	    goLogger.debug("Attachment is saved at location: "+f.getAbsolutePath());
	}
	
}
